#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CodeDrift.EditorTools
{
    /// <summary>
    /// Editor-only utility: buffers every Console log emitted while Play Mode is active and writes a
    /// timestamped .log file (outside Assets/) when Play Mode exits. No runtime dependency.
    /// </summary>
    [InitializeOnLoad]
    public static class PlayModeLogExporter
    {
        // ---- Configuration ----------------------------------------------------------------
        /// <summary>Title printed in the log header.</summary>
        public const string ProjectLabel = "TANK MAYHEM";
        /// <summary>Output directory, relative to the project root (the folder that contains Assets/).</summary>
        public const string OutputDirectoryRelative = "Logs/PlayMode";
        /// <summary>Safety cap so a log storm cannot exhaust Editor memory. Counts keep increasing past it.</summary>
        const int k_MaxBufferedEntries = 200000;

        const string k_LastExportPrefKey = "CodeDrift.PlayModeLogExporter.LastExport";

        struct Entry
        {
            public DateTime Time;
            public LogType Type;
            public string Message;
            public string StackTrace;
        }

        // ---- State (static; reset on domain reload, which is fine because we export before it) ----
        static readonly object s_Lock = new object();
        static readonly List<Entry> s_Entries = new List<Entry>(1024);
        static bool s_Capturing;
        static DateTime s_SessionStart;
        static int s_Total, s_Log, s_Warning, s_Error, s_Exception, s_Assert, s_Dropped;
        static int s_CallbackInvocations;
        static string s_SubscribedAt = "-", s_StartedAt = "-";

        // Capture intent survives Domain Reload through SessionState (static fields do not).
        const string k_ActiveKey = "CodeDrift.PlayModeLogExporter.Active";
        const string k_StartTicksKey = "CodeDrift.PlayModeLogExporter.StartTicks";

        /// <summary>Prints one summary line to the Console after each export (callback/buffer counts). Set false to silence.</summary>
        const bool k_EnableDiagnostics = true;

        public static string OutputDirectory
        {
            get
            {
                string projectRoot = Directory.GetParent(Application.dataPath).FullName;
                return Path.GetFullPath(Path.Combine(projectRoot, OutputDirectoryRelative));
            }
        }

        public static string LastExportedFile
        {
            get { try { return EditorPrefs.GetString(k_LastExportPrefKey, string.Empty); } catch { return string.Empty; } }
        }

        public static int ExportedSessionCount
        {
            get
            {
                try
                {
                    string dir = OutputDirectory;
                    return Directory.Exists(dir) ? Directory.GetFiles(dir, "PlayMode_*.log").Length : 0;
                }
                catch { return 0; }
            }
        }

        static PlayModeLogExporter()
        {
            // Unsubscribe first so repeated static-constructor runs (or a manual re-init) never double-subscribe.
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

            // Domain Reload on entering Play Mode re-runs this constructor BEFORE the scene's Awake/OnEnable/Start.
            // Resume the session that ExitingEditMode started so those early logs are captured.
            if (SessionState.GetBool(k_ActiveKey, false))
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    BeginSession(new DateTime(long.Parse(SessionState.GetString(k_StartTicksKey, DateTime.Now.Ticks.ToString())), DateTimeKind.Local));
                else
                    SessionState.SetBool(k_ActiveKey, false); // stale intent (e.g. entering Play Mode was cancelled)
            }
            else if (EditorApplication.isPlaying)
            {
                BeginSession(DateTime.Now); // domain reload disabled and Editor already playing
            }
        }

        // ---- Lifecycle -----------------------------------------------------------------
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    // Earliest hook: fires before the optional domain reload and before any runtime script runs.
                    BeginSession(DateTime.Now);
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    BeginSession(DateTime.Now); // no-op if already capturing
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    // Must finish here: with Domain Reload enabled the static buffer is wiped before EnteredEditMode.
                    EndSessionAndExport();
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    // Safety net / guarantees no capture or stale intent in Edit Mode.
                    EndSessionAndExport();
                    SessionState.SetBool(k_ActiveKey, false);
                    break;
            }
        }

        static void OnBeforeAssemblyReload()
        {
            if (EditorApplication.isPlaying)
            {
                // Script recompile during Play Mode: save what we have.
                EndSessionAndExport();
            }
            else if (s_Capturing)
            {
                // Domain reload while ENTERING Play Mode: the buffer would be wiped anyway (only Editor logs so far).
                // Drop it silently without exporting; the constructor resumes capture from SessionState afterwards.
                lock (s_Lock) { s_Capturing = false; s_Entries.Clear(); }
                Application.logMessageReceivedThreaded -= OnLogMessage;
            }
        }

        static void BeginSession(DateTime start)
        {
            lock (s_Lock)
            {
                if (s_Capturing) return; // prevents duplicate subscriptions / sessions
                s_Entries.Clear();
                s_Total = s_Log = s_Warning = s_Error = s_Exception = s_Assert = s_Dropped = 0;
                s_CallbackInvocations = 0;
                s_SessionStart = start;
                s_Capturing = true;
            }
            SessionState.SetBool(k_ActiveKey, true);
            SessionState.SetString(k_StartTicksKey, start.Ticks.ToString());

            // Threaded variant also catches logs raised from worker threads (e.g. Firebase async callbacks).
            Application.logMessageReceivedThreaded -= OnLogMessage;
            Application.logMessageReceivedThreaded += OnLogMessage;
            s_SubscribedAt = s_StartedAt = DateTime.Now.ToString("HH:mm:ss.fff");
        }

        static void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            // Keep this cheap and non-logging: no file I/O, no Debug.* calls (avoids recursion).
            lock (s_Lock)
            {
                if (!s_Capturing) return;

                s_CallbackInvocations++;
                s_Total++;
                switch (type)
                {
                    case LogType.Log: s_Log++; break;
                    case LogType.Warning: s_Warning++; break;
                    case LogType.Error: s_Error++; break;
                    case LogType.Exception: s_Exception++; break;
                    case LogType.Assert: s_Assert++; break;
                }

                if (s_Entries.Count >= k_MaxBufferedEntries) { s_Dropped++; return; }
                s_Entries.Add(new Entry { Time = DateTime.Now, Type = type, Message = condition, StackTrace = stackTrace });
            }
        }

        // ---- Export --------------------------------------------------------------------
        static void EndSessionAndExport()
        {
            Entry[] entries;
            DateTime start, end;
            int total, log, warn, err, exc, assert, dropped, invocations;

            lock (s_Lock)
            {
                if (!s_Capturing) return; // already exported: no multiple exports per session
                s_Capturing = false;
                entries = s_Entries.ToArray();
                s_Entries.Clear();
                start = s_SessionStart;
                end = DateTime.Now;
                invocations = s_CallbackInvocations;
                total = s_Total; log = s_Log; warn = s_Warning; err = s_Error; exc = s_Exception; assert = s_Assert; dropped = s_Dropped;
            }
            Application.logMessageReceivedThreaded -= OnLogMessage;
            SessionState.SetBool(k_ActiveKey, false);

            try
            {
                string text = BuildReport(entries, start, end, total, log, warn, err, exc, assert, dropped);
                string path = WriteUnique(text, start);
                EditorPrefs.SetString(k_LastExportPrefKey, path);
                Debug.Log("[PlayModeLogExporter] Saved " + entries.Length + " entries to " + path); // capture already stopped
                if (k_EnableDiagnostics)
                    Debug.Log("[PlayModeLogExporter][DIAG] subscribed/started " + s_StartedAt + ", stopped " + end.ToString("HH:mm:ss.fff")
                        + ", callback invocations " + invocations + ", buffered " + entries.Length);
            }
            catch (Exception e)
            {
                // One failed export must never break the Editor.
                Debug.LogWarning("[PlayModeLogExporter] Export failed: " + e.Message);
            }
        }

        static string BuildReport(Entry[] entries, DateTime start, DateTime end,
            int total, int log, int warn, int err, int exc, int assert, int dropped)
        {
            const string bar = "==================================================";
            var sb = new StringBuilder(64 * 1024);

            sb.AppendLine(bar);
            sb.AppendLine(ProjectLabel + " - PLAY MODE LOG");
            sb.AppendLine(bar);
            sb.AppendLine("Session Start: " + start.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("Unity Version: " + Application.unityVersion);
            sb.AppendLine("Platform: " + EditorUserBuildSettings.activeBuildTarget + " (Editor: " + Application.platform + ")");
            sb.AppendLine(bar);
            sb.AppendLine();

            foreach (Entry e in entries)
            {
                sb.Append('[').Append(e.Time.ToString("HH:mm:ss.fff")).Append("] [").Append(Label(e.Type)).AppendLine("]");
                sb.AppendLine(e.Message ?? string.Empty);
                if (!string.IsNullOrWhiteSpace(e.StackTrace))
                {
                    sb.AppendLine();
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine(e.StackTrace.TrimEnd());
                }
                sb.AppendLine();
            }

            TimeSpan duration = end - start;
            sb.AppendLine(bar);
            sb.AppendLine("SESSION SUMMARY");
            sb.AppendLine(bar);
            sb.AppendLine("Session End: " + end.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("Duration: " + duration.ToString(@"hh\:mm\:ss\.fff"));
            sb.AppendLine("Total Log Entries: " + total);
            sb.AppendLine("Log Count: " + log);
            sb.AppendLine("Warning Count: " + warn);
            sb.AppendLine("Error Count: " + err);
            sb.AppendLine("Exception Count: " + exc);
            if (assert > 0) sb.AppendLine("Assert Count: " + assert);
            if (dropped > 0) sb.AppendLine("Entries not stored (buffer cap " + k_MaxBufferedEntries + "): " + dropped);
            sb.AppendLine(bar);
            return sb.ToString();
        }

        static string Label(LogType type)
        {
            switch (type)
            {
                case LogType.Log: return "LOG";
                case LogType.Warning: return "WARNING";
                case LogType.Error: return "ERROR";
                case LogType.Exception: return "EXCEPTION";
                case LogType.Assert: return "ASSERT";
                default: return type.ToString().ToUpperInvariant();
            }
        }

        static string WriteUnique(string text, DateTime start)
        {
            string dir = OutputDirectory;
            Directory.CreateDirectory(dir);
            string baseName = "PlayMode_" + start.ToString("yyyy-MM-dd_HH-mm-ss");
            var utf8 = new UTF8Encoding(false);

            for (int i = 0; i < 1000; i++)
            {
                string path = Path.Combine(dir, i == 0 ? baseName + ".log" : baseName + "_" + i + ".log");
                try
                {
                    // CreateNew fails if the file exists, so a previous session is never overwritten.
                    using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                    using (var w = new StreamWriter(fs, utf8))
                    {
                        w.Write(text);
                        w.Flush();
                        fs.Flush(true);
                    }
                    return path;
                }
                catch (IOException) when (File.Exists(path)) { /* name taken: try next suffix */ }
            }
            throw new IOException("Could not find a free log file name in " + dir);
        }

        // ---- Menu / UI -----------------------------------------------------------------
        [MenuItem("Tools/Tank Mayhem/Open Play Mode Logs Folder")]
        public static void OpenLogsFolder()
        {
            try
            {
                string dir = OutputDirectory;
                Directory.CreateDirectory(dir);
                EditorUtility.RevealInFinder(dir);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[PlayModeLogExporter] Could not open logs folder: " + e.Message);
            }
        }

        [MenuItem("Tools/Tank Mayhem/Play Mode Log Exporter")]
        static void ShowWindow()
        {
            var w = EditorWindow.GetWindow<ExporterWindow>(true, "Play Mode Log Exporter");
            w.minSize = new Vector2(420, 110);
        }

        sealed class ExporterWindow : EditorWindow
        {
            void OnGUI()
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Export directory", OutputDirectory);
                string last = LastExportedFile;
                EditorGUILayout.LabelField("Last exported", string.IsNullOrEmpty(last) ? "(none this Editor install)" : Path.GetFileName(last));
                EditorGUILayout.LabelField("Exported sessions", ExportedSessionCount.ToString());
                EditorGUILayout.Space();
                if (GUILayout.Button("Open Logs Folder")) OpenLogsFolder();
            }
        }
    }
}
#endif
