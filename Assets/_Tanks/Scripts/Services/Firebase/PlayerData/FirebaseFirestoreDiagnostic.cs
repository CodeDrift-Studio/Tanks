#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

namespace Tanks.Complete.FirebaseSystem
{
    public enum DiagnosticStepResult
    {
        NotRun,
        Passed,
        /// <summary>The task COMPLETED with an exception or was cancelled.</summary>
        Failed,
        /// <summary>The diagnostic stopped waiting; the task is still pending and may yet succeed or fail. NOT a failure verdict.</summary>
        TimedOutPending
    }

    /// <summary>Outcome of every step of one diagnostic run. Success only if ALL required steps passed.</summary>
    public class FirestoreDiagnosticReport
    {
        public string DocumentPath;
        public DiagnosticStepResult Preconditions = DiagnosticStepResult.NotRun;
        public DiagnosticStepResult Write = DiagnosticStepResult.NotRun;
        public DiagnosticStepResult Read = DiagnosticStepResult.NotRun;
        public DiagnosticStepResult Verify = DiagnosticStepResult.NotRun;
        public DiagnosticStepResult Delete = DiagnosticStepResult.NotRun;
        /// <summary>True = document known to remain; False = known deleted/never created; null = cannot be determined (a write is pending).</summary>
        public bool? DocumentRemains;
        public string FailureReason;
        /// <summary>Last Firestore message the native SDK printed during the run (e.g. the real server error behind a pending task).</summary>
        public string LastNativeMessage;

        public bool Success => Preconditions == DiagnosticStepResult.Passed && Write == DiagnosticStepResult.Passed
            && Read == DiagnosticStepResult.Passed && Verify == DiagnosticStepResult.Passed
            && Delete == DiagnosticStepResult.Passed && DocumentRemains == false;
    }

    /// <summary>
    /// MANUAL Firestore connectivity + security check. Not player-data sync; never runs automatically.
    /// Writes diagnostics/{uid}, reads it back from the SERVER (not the local cache), compares the fields and deletes it.
    /// Trigger: Editor menu Tools > Firebase > Run Firestore Diagnostic (Play Mode), or call <see cref="Run"/>.
    /// Compiled only in the Editor and Development builds. Log lines use [FIREBASE][FIRESTORE].
    ///
    /// Timeout semantics: a timeout means "we stopped waiting", NOT "the operation failed". The SDK keeps a write queued and
    /// retries it (an API-disabled error, for example, is retried silently), so the task is tracked until it really completes
    /// and its late result is logged. While a write is pending no second operation is started on that document: no
    /// cleanup delete, no new run.
    /// </summary>
    public class FirebaseFirestoreDiagnostic : MonoBehaviour
    {
        public const string CollectionName = "diagnostics";
        private const float k_StepTimeoutSeconds = 30f;
        private const string k_Tag = "Diagnostic: ";

        private static bool s_Running;
        // The write of an earlier run that has not completed yet. Blocks new runs so no duplicate write is queued.
        private static Task s_PendingWrite;
        private static string s_PendingWritePath;

        // Native SDK messages arrive on a worker thread; keep only the newest relevant one.
        private static readonly object s_NativeLock = new object();
        private static string s_LastNative;

        /// <summary>Starts one run. The callback (optional) receives the final report on the main thread.</summary>
        public static void Run(Action<FirestoreDiagnosticReport> onDone = null)
        {
            if (s_Running)
            {
                FirebaseLogger.Warning(FirebaseLogger.Firestore, k_Tag + "already running; request ignored.");
                return;
            }
            if (s_PendingWrite != null && !s_PendingWrite.IsCompleted)
            {
                FirebaseLogger.Warning(FirebaseLogger.Firestore, k_Tag + "NOT started: the write to " + s_PendingWritePath
                    + " from an earlier run is still pending. Wait for its 'completed late' log line (or stop Play Mode) to avoid a duplicate write.");
                return;
            }
            s_Running = true;
            var host = new GameObject("FirestoreDiagnostic (temporary)");
            host.hideFlags = HideFlags.HideAndDontSave;
            var runner = host.AddComponent<FirebaseFirestoreDiagnostic>();
            runner.StartCoroutine(runner.Execute(onDone));
        }

        private static void OnNativeLog(string condition, string stackTrace, LogType type)
        {
            if (condition == null) return;
            if (condition.Contains("WriteStream") || condition.Contains("Write at") || condition.Contains("Firestore")
                || condition.Contains("Stream error"))
            {
                // Strip nothing of value; these lines contain project ids and server messages, never tokens.
                lock (s_NativeLock) s_LastNative = condition.Length > 400 ? condition.Substring(0, 400) : condition;
            }
        }

        private static string TakeLastNative()
        {
            lock (s_NativeLock) return s_LastNative;
        }

        private IEnumerator Execute(Action<FirestoreDiagnosticReport> onDone)
        {
            var report = new FirestoreDiagnosticReport();
            lock (s_NativeLock) s_LastNative = null;
            Application.logMessageReceivedThreaded += OnNativeLog;
            try
            {
                yield return Sequence(report);
            }
            finally
            {
                Application.logMessageReceivedThreaded -= OnNativeLog;
            }
            report.LastNativeMessage = TakeLastNative();
            LogSummary(report);
            s_Running = false;
            onDone?.Invoke(report);
            Destroy(gameObject);
        }

        private IEnumerator Sequence(FirestoreDiagnosticReport report)
        {
            // ---- 1/2. Preconditions. Nothing touches Firestore until all pass. ----
            if (!FirebaseManager.IsReady)
            {
                Fail(report, "Firebase is not initialized (status=" + FirebaseManager.Status + ").");
                report.Preconditions = DiagnosticStepResult.Failed;
                report.DocumentRemains = false;
                yield break;
            }
            Pass("Step 1 Firebase initialized (status=" + FirebaseManager.Status + ").");

            string uid = FirebaseManager.UserId;
            if (!FirebaseManager.IsSignedIn || string.IsNullOrEmpty(uid))
            {
                Fail(report, "Authentication missing (AuthStatus=" + FirebaseManager.AuthStatus + ", uidPresent=" + !string.IsNullOrEmpty(uid) + "). No Firestore call made.");
                report.Preconditions = DiagnosticStepResult.Failed;
                report.DocumentRemains = false;
                yield break;
            }
            Pass("Step 2 authenticated. uid=" + uid + " anonymous=" + FirebaseManager.IsAnonymousAccount);

            DocumentReference doc;
            try
            {
                var db = FirebaseFirestore.DefaultInstance;
                if (db == null) throw new InvalidOperationException("FirebaseFirestore.DefaultInstance is null.");
                doc = db.Collection(CollectionName).Document(uid);
                Pass("Firestore instance acquired. projectId=" + Firebase.FirebaseApp.DefaultInstance.Options.ProjectId + " path=" + CollectionName + "/" + uid);
            }
            catch (Exception e)
            {
                Fail(report, "Firestore instance unavailable. " + FirebaseLogger.Describe(e));
                report.Preconditions = DiagnosticStepResult.Failed;
                report.DocumentRemains = false;
                yield break;
            }
            report.Preconditions = DiagnosticStepResult.Passed;
            report.DocumentPath = CollectionName + "/" + uid;
            report.DocumentRemains = false;

            // ---- 3. Write ----
            string testId = Guid.NewGuid().ToString("N");
            string message = "tank-mayhem firestore connectivity test";
            string createdAt = DateTime.UtcNow.ToString("o");
            var data = new Dictionary<string, object>
            {
                { "test_id", testId },
                { "created_at", createdAt },
                { "test_message", message },
                { "authenticated_uid", uid },
            };

            Task writeTask = null;
            try
            {
                writeTask = doc.SetAsync(data);
                s_PendingWrite = writeTask;
                s_PendingWritePath = report.DocumentPath;
            }
            catch (Exception e) { report.Write = DiagnosticStepResult.Failed; Fail(report, "Write threw synchronously. " + FirebaseLogger.Describe(e)); }

            if (writeTask != null)
            {
                var sw = Stopwatch.StartNew();
                string startedAt = DateTime.UtcNow.ToString("HH:mm:ss.fff");
                Info("Step 3 write STARTED at " + startedAt + "Z (timeout " + k_StepTimeoutSeconds + "s). test_id=" + testId);
                TrackCompletion(writeTask, "write", report.DocumentPath, sw, true);

                yield return WaitFor(writeTask);
                if (!writeTask.IsCompleted)
                {
                    report.Write = DiagnosticStepResult.TimedOutPending;
                    report.DocumentRemains = null;
                    Fail(report, "Write TIMED OUT after " + sw.Elapsed.TotalSeconds.ToString("F1") + "s: the task is STILL PENDING, not failed. "
                        + "The SDK is retrying it in the background; its real result is logged when it completes. "
                        + "Last SDK message: " + (TakeLastNative() ?? "none"));
                }
                else if (writeTask.IsFaulted || writeTask.IsCanceled)
                {
                    report.Write = DiagnosticStepResult.Failed;
                    Fail(report, "Write task COMPLETED WITH ERROR after " + sw.Elapsed.TotalSeconds.ToString("F1") + "s. " + Classify(writeTask));
                }
                else
                {
                    report.Write = DiagnosticStepResult.Passed;
                    report.DocumentRemains = true;
                    Pass("Step 3 write COMPLETED successfully after " + sw.Elapsed.TotalSeconds.ToString("F1") + "s (server acknowledged).");
                }
            }

            // ---- 4/5. Read back from the server, compare (only after a confirmed write) ----
            if (report.Write == DiagnosticStepResult.Passed)
            {
                Task<DocumentSnapshot> readTask = null;
                try { readTask = doc.GetSnapshotAsync(Source.Server); }
                catch (Exception e) { report.Read = DiagnosticStepResult.Failed; Fail(report, "Read threw synchronously. " + FirebaseLogger.Describe(e)); }

                if (readTask != null)
                {
                    var sw = Stopwatch.StartNew();
                    Info("Step 4 read STARTED at " + DateTime.UtcNow.ToString("HH:mm:ss.fff") + "Z (source=server).");
                    TrackCompletion(readTask, "read", report.DocumentPath, sw, false);
                    yield return WaitFor(readTask);
                    if (!readTask.IsCompleted)
                    {
                        report.Read = DiagnosticStepResult.TimedOutPending;
                        Fail(report, "Read TIMED OUT after " + sw.Elapsed.TotalSeconds.ToString("F1") + "s: task still pending (not a failure verdict). Last SDK message: " + (TakeLastNative() ?? "none"));
                    }
                    else if (readTask.IsFaulted || readTask.IsCanceled)
                    {
                        report.Read = DiagnosticStepResult.Failed;
                        Fail(report, "Read task COMPLETED WITH ERROR. " + Classify(readTask));
                    }
                    else
                    {
                        var snap = readTask.Result;
                        if (!snap.Exists)
                        {
                            report.Read = DiagnosticStepResult.Failed;
                            Fail(report, "Read returned no document although the write succeeded.");
                        }
                        else
                        {
                            report.Read = DiagnosticStepResult.Passed;
                            Pass("Step 4 read COMPLETED successfully after " + sw.Elapsed.TotalSeconds.ToString("F1") + "s.");
                            report.Verify = Verify(snap, testId, createdAt, message, uid, report);
                        }
                    }
                }
            }

            // ---- 6. Cleanup: only when the document is KNOWN to exist. While the write is pending nothing else is sent. ----
            if (report.Write == DiagnosticStepResult.Passed)
            {
                Task deleteTask = null;
                try { deleteTask = doc.DeleteAsync(); }
                catch (Exception e) { report.Delete = DiagnosticStepResult.Failed; report.DocumentRemains = true; Fail(report, "Delete threw synchronously. " + FirebaseLogger.Describe(e)); }

                if (deleteTask != null)
                {
                    var sw = Stopwatch.StartNew();
                    Info("Step 6 delete STARTED at " + DateTime.UtcNow.ToString("HH:mm:ss.fff") + "Z.");
                    TrackCompletion(deleteTask, "delete", report.DocumentPath, sw, false);
                    yield return WaitFor(deleteTask);
                    if (!deleteTask.IsCompleted)
                    {
                        report.Delete = DiagnosticStepResult.TimedOutPending;
                        report.DocumentRemains = null;
                        Fail(report, "Delete TIMED OUT after " + sw.Elapsed.TotalSeconds.ToString("F1") + "s: task still pending; document state unknown.");
                    }
                    else if (deleteTask.IsFaulted || deleteTask.IsCanceled)
                    {
                        report.Delete = DiagnosticStepResult.Failed;
                        report.DocumentRemains = true;
                        Fail(report, "Delete task COMPLETED WITH ERROR. Document diagnostics/{uid} REMAINS. " + Classify(deleteTask));
                    }
                    else
                    {
                        report.Delete = DiagnosticStepResult.Passed;
                        report.DocumentRemains = false;
                        Pass("Step 6 delete COMPLETED successfully after " + sw.Elapsed.TotalSeconds.ToString("F1") + "s. Temporary document removed.");
                    }
                }
            }
            else if (report.Write == DiagnosticStepResult.TimedOutPending)
            {
                Info("Step 6 cleanup SKIPPED on purpose: the write is still pending, so no second operation is sent to the same document.");
            }
        }

        // Logs the real completion of a task even after the diagnostic gave up waiting (the host object may be gone by then).
        private static void TrackCompletion(Task task, string op, string path, Stopwatch sw, bool isWrite)
        {
            task.ContinueWithOnMainThread(t =>
            {
                string status = "IsCompleted=" + t.IsCompleted + " IsFaulted=" + t.IsFaulted + " IsCanceled=" + t.IsCanceled;
                string when = DateTime.UtcNow.ToString("HH:mm:ss.fff") + "Z after " + sw.Elapsed.TotalSeconds.ToString("F1") + "s";
                bool late = sw.Elapsed.TotalSeconds > k_StepTimeoutSeconds;
                string prefix = op + (late ? " COMPLETED LATE (after the diagnostic stopped waiting)" : " task completed") + " at " + when + ". " + status + ". ";
                if (t.IsFaulted || t.IsCanceled)
                    FirebaseLogger.Warning(FirebaseLogger.Firestore, k_Tag + prefix + Classify(t));
                else
                {
                    FirebaseLogger.Info(FirebaseLogger.Firestore, k_Tag + prefix + "Result: SUCCESS.");
                    if (isWrite && late)
                        FirebaseLogger.Warning(FirebaseLogger.Firestore, k_Tag + "The late write SUCCEEDED, so " + path
                            + " now EXISTS on the server. Run the diagnostic again (it overwrites then deletes it) or delete it in the Console.");
                }
                if (isWrite && ReferenceEquals(s_PendingWrite, task)) s_PendingWrite = null;
            });
        }

        private DiagnosticStepResult Verify(DocumentSnapshot snap, string testId, string createdAt, string message, string uid, FirestoreDiagnosticReport report)
        {
            var mismatches = new List<string>();
            Compare(snap, "test_id", testId, mismatches);
            Compare(snap, "created_at", createdAt, mismatches);
            Compare(snap, "test_message", message, mismatches);
            Compare(snap, "authenticated_uid", uid, mismatches);
            if (mismatches.Count == 0)
            {
                Pass("Step 5 data verified: all 4 fields match.");
                return DiagnosticStepResult.Passed;
            }
            Fail(report, "Data verification FAILED: " + string.Join("; ", mismatches));
            return DiagnosticStepResult.Failed;
        }

        private static void Compare(DocumentSnapshot snap, string field, string expected, List<string> mismatches)
        {
            string actual = null;
            try { if (snap.TryGetValue(field, out string v)) actual = v; } catch { }
            if (actual != expected) mismatches.Add(field + " expected='" + expected + "' actual='" + (actual ?? "missing") + "'");
        }

        // Polls instead of blocking so the main thread keeps running; stops waiting after the timeout.
        private static IEnumerator WaitFor(Task task)
        {
            float end = Time.realtimeSinceStartup + k_StepTimeoutSeconds;
            while (!task.IsCompleted && Time.realtimeSinceStartup < end) yield return null;
        }

        // Describes a COMPLETED faulted/cancelled task: Firestore error code and message, with the cause to act on.
        private static string Classify(Task task)
        {
            if (task.IsCanceled && task.Exception == null) return "Task was cancelled.";
            var e = task.Exception?.GetBaseException();
            string text = FirebaseLogger.Describe(task.Exception);
            if (e is FirestoreException fe)
            {
                switch (fe.ErrorCode)
                {
                    case FirestoreError.PermissionDenied:
                        return "PERMISSION DENIED (security rules reject this access, or the Cloud Firestore API is disabled/not initialized for the project). " + text;
                    case FirestoreError.Unavailable:
                    case FirestoreError.DeadlineExceeded:
                        return "BACKEND UNREACHABLE / network unavailable. " + text;
                    case FirestoreError.NotFound:
                        return "NOT FOUND: the Firestore database probably does not exist. " + text;
                    case FirestoreError.Unauthenticated:
                        return "UNAUTHENTICATED: no valid auth token on the request. " + text;
                }
            }
            return text;
        }

        private static void Info(string message) => FirebaseLogger.Info(FirebaseLogger.Firestore, k_Tag + message);
        private static void Pass(string message) => FirebaseLogger.Info(FirebaseLogger.Firestore, k_Tag + message);

        private static void Fail(FirestoreDiagnosticReport report, string message)
        {
            if (report.FailureReason == null) report.FailureReason = message;
            FirebaseLogger.Error(FirebaseLogger.Firestore, k_Tag + message);
        }

        private static void LogSummary(FirestoreDiagnosticReport r)
        {
            string remains = r.DocumentRemains.HasValue ? (r.DocumentRemains.Value ? "YES" : "no") : "UNKNOWN (write still pending)";
            string line = k_Tag + "RESULT=" + (r.Success ? "SUCCESS" : "NOT VERIFIED") + " path=" + (r.DocumentPath ?? "n/a")
                + " | preconditions=" + r.Preconditions + " write=" + r.Write + " read=" + r.Read + " verify=" + r.Verify
                + " delete=" + r.Delete + " | documentRemains=" + remains
                + (r.FailureReason != null ? " | firstProblem=" + r.FailureReason : "");
            if (r.Success) FirebaseLogger.Info(FirebaseLogger.Firestore, line);
            else FirebaseLogger.Error(FirebaseLogger.Firestore, line);
        }
    }
}
#endif
