using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>
    /// The single entry point for Firebase in game code. Gameplay scripts call the static Report*/Set* methods here
    /// and never reference the Firebase SDK. Every method is a safe no-op when Firebase is unavailable.
    ///
    /// Lifecycle: the host object is created automatically before the first scene loads (no Boot scene needed),
    /// survives scene reloads (DontDestroyOnLoad) and a duplicate is destroyed. Initialization is asynchronous and
    /// never blocks gameplay.
    ///
    /// Privacy: data collection (Analytics + Crashlytics) follows one switch, persisted in PlayerPrefs.
    /// Call <see cref="SetDataCollectionEnabled"/> from a future consent/settings screen. The default is
    /// <see cref="k_DefaultCollectionEnabled"/> (PENDING DECISION: audience/age group and consent UX).
    /// </summary>
    public class FirebaseManager : MonoBehaviour
    {
        private const bool k_DefaultCollectionEnabled = true;
        private const string k_CollectionPrefKey = "firebase_data_collection";

        private static FirebaseManager s_Instance;

        public static FirebaseStatus Status { get; private set; } = FirebaseStatus.NotStarted;
        public static bool IsReady => Status == FirebaseStatus.Ready;

        /// <summary>Raised on the main thread when initialization finishes (Ready or Failed).</summary>
        public static event Action<FirebaseStatus> StatusChanged;

        private FirebaseAnalyticsHandler m_Analytics;
        private FirebaseCrashlyticsHandler m_Crashlytics;
        private FirebaseRemoteConfigHandler m_RemoteConfig;
        private FirebaseAuthenticationHandler m_Auth;
        private FirebasePlayerDataHandler m_PlayerData;

        /// <summary>
        /// Raw access to players/{uid}. Schema-free and NOT synced automatically; nothing calls it yet.
        /// Null before the manager exists. Check the result status of each call (Unavailable / NotSignedIn are normal).
        /// </summary>
        public static FirebasePlayerDataHandler PlayerData => s_Instance != null ? s_Instance.m_PlayerData : null;

        private const float k_AuthRetrySeconds = 20f;
        private const float k_WatchdogSeconds = 15f;

        // ----- Authentication (exposed centrally; no SDK types leak to game code) -----
        // Never looks "healthy" when Firebase itself failed: Unavailable is reported instead of a stale NotStarted.
        public static FirebaseAuthStatus AuthStatus
        {
            get
            {
                if (s_Instance == null) return FirebaseAuthStatus.NotStarted;
                if (Status == FirebaseStatus.Failed) return FirebaseAuthStatus.Unavailable;
                return s_Instance.m_Auth.Status;
            }
        }
        public static bool IsSignedIn => AuthStatus == FirebaseAuthStatus.SignedIn;
        /// <summary>Firebase UID or null. Pseudonymous: do not send to Analytics or Crashlytics.</summary>
        public static string UserId => s_Instance != null ? s_Instance.m_Auth.UserId : null;
        public static bool IsAnonymousAccount => s_Instance != null && s_Instance.m_Auth.IsAnonymous;
        /// <summary>Raised on the main thread when the sign-in state changes (new status, UID or null).</summary>
        public static event Action<FirebaseAuthStatus, string> AuthStatusChanged;
        /// <summary>For the future GPGS flow: links a credential to the current anonymous account, keeping its UID.</summary>
        public static FirebaseAuthenticationHandler Authentication => s_Instance != null ? s_Instance.m_Auth : null;

        // Handlers are never null so callers can read RemoteConfig defaults even without Firebase.
        public static FirebaseRemoteConfigHandler RemoteConfig => s_Instance != null ? s_Instance.m_RemoteConfig : s_FallbackRemoteConfig;
        private static readonly FirebaseRemoteConfigHandler s_FallbackRemoteConfig = new FirebaseRemoteConfigHandler();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (s_Instance != null) return;
            var go = new GameObject("FirebaseManager");
            go.AddComponent<FirebaseManager>();
        }

        // Domain reload may be disabled in the Editor; reset statics so Play Mode starts clean.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_Instance = null;
            Status = FirebaseStatus.NotStarted;
            StatusChanged = null;
            AuthStatusChanged = null;
        }

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            bool collect = IsDataCollectionEnabled;
            m_Analytics = new FirebaseAnalyticsHandler(collect);
            m_Crashlytics = new FirebaseCrashlyticsHandler(collect);
            m_RemoteConfig = new FirebaseRemoteConfigHandler();
            m_Auth = new FirebaseAuthenticationHandler();
            m_Auth.StatusChanged += OnAuthHandlerStatusChanged;
            m_PlayerData = new FirebasePlayerDataHandler(() => m_Auth.UserId);

            SceneManager.sceneLoaded += OnSceneLoaded;
            FirebaseLogger.Info(FirebaseLogger.Core, "FirebaseManager created (persistent host). status=" + Status);

            if (Status != FirebaseStatus.NotStarted)
            {
                FirebaseLogger.Warning(FirebaseLogger.Core, "Initialization NOT started again: status is already " + Status);
                return; // never initialize twice
            }
            Status = FirebaseStatus.Initializing;
            FirebaseInitializer.Run(OnInitialized, OnInitializeFailed);
        }

        private void OnDestroy()
        {
            if (s_Instance != this) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            s_Instance = null;
        }

        private void OnInitialized()
        {
            // Status is Ready BEFORE services activate so any handler reading it during activation sees the truth.
            Status = FirebaseStatus.Ready;
            FirebaseLogger.Info(FirebaseLogger.Init, "Initialization SUCCEEDED. Activating services.");

            // Crashlytics first so problems in the other services are reportable.
            m_Crashlytics.Activate();
            m_Analytics.Activate();
            m_RemoteConfig.Activate();
            m_Auth.Activate();   // restores the saved session, or signs in anonymously once
            m_PlayerData.Activate();
            StartCoroutine(AuthRetryLoop());
            StartCoroutine(AuthWatchdog());

            m_Analytics.FlushBuffer();
            LogDiagnostics("after service activation");
            StatusChanged?.Invoke(Status);
        }

        private void OnInitializeFailed(string reason)
        {
            Status = FirebaseStatus.Failed;
            m_Analytics.MarkFailed();
            m_Auth.MarkUnavailable();
            FirebaseLogger.Error(FirebaseLogger.Init, "Initialization FAILED; Firebase is unavailable for this session. " + reason);
            StatusChanged?.Invoke(Status);
        }

        // Observes the auth handler and double-checks that the UID really reaches the manager's public API.
        private void OnAuthHandlerStatusChanged(FirebaseAuthStatus status, string uid)
        {
            FirebaseLogger.Info(FirebaseLogger.Auth, "Manager view: AuthStatus=" + AuthStatus + " IsSignedIn=" + IsSignedIn
                + " UserId=" + (UserId ?? "null") + " IsAnonymousAccount=" + IsAnonymousAccount);

            if (status == FirebaseAuthStatus.SignedIn && (string.IsNullOrEmpty(UserId) || UserId != uid))
                FirebaseLogger.Error(FirebaseLogger.Auth, "UID NOT propagated: handler reported SignedIn but manager UserId='" + (UserId ?? "null") + "'.");

            AuthStatusChanged?.Invoke(status, uid);
        }

        /// <summary>
        /// Logs one summary of the whole Firebase state. Call it any time (e.g. from a debug button) when diagnosing.
        /// Includes project/app ids (not secrets) to compare with the Firebase Console.
        /// </summary>
        public static void LogDiagnostics(string reason)
        {
            if (s_Instance == null) { FirebaseLogger.Warning(FirebaseLogger.Core, "Diagnostics (" + reason + "): no FirebaseManager instance."); return; }
            var h = s_Instance;
            string project = "n/a";
            try
            {
                if (IsReady) project = "projectId=" + Firebase.FirebaseApp.DefaultInstance.Options.ProjectId;
            }
            catch (Exception e) { project = "error:" + e.GetType().Name; }

            FirebaseLogger.Warning(FirebaseLogger.Core, "DIAGNOSTICS (" + reason + "): firebase=" + Status + " " + project
                + " | auth: " + h.m_Auth.DescribeState() + " | managerAuthStatus=" + AuthStatus
                + " | analytics=" + h.m_Analytics.IsAvailable + " crashlytics=" + h.m_Crashlytics.IsAvailable
                + " remoteConfig=" + h.m_RemoteConfig.IsAvailable + " firestore=" + h.m_PlayerData.IsAvailable
                + " | network=" + Application.internetReachability + " package=" + Application.identifier);
        }

        // One-shot check shortly after startup: separates "auth never started" from "sign-in failed" from "all good".
        private System.Collections.IEnumerator AuthWatchdog()
        {
            yield return new WaitForSecondsRealtime(k_WatchdogSeconds);
            if (m_Auth.Status == FirebaseAuthStatus.SignedIn)
            {
                FirebaseLogger.Info(FirebaseLogger.Auth, "Watchdog: signed in after " + k_WatchdogSeconds + "s. OK.");
                yield break;
            }

            string verdict;
            if (!m_Auth.IsAvailable) verdict = "A: auth handler never became available (see activation errors above).";
            else if (m_Auth.SignInAttempts == 0) verdict = "A: Firebase ready but NO sign-in request was ever sent.";
            else if (m_Auth.LastError != null) verdict = "B: sign-in was requested but failed: " + m_Auth.LastError;
            else verdict = "B?: sign-in requested but no result yet (network/Google Play services slow or blocked?).";
            LogDiagnostics("watchdog " + k_WatchdogSeconds + "s, NOT signed in. " + verdict);
        }

        // Retries sign-in only while signed out and the device reports a connection; sleeps otherwise (no per-frame work).
        private System.Collections.IEnumerator AuthRetryLoop()
        {
            var wait = new WaitForSecondsRealtime(k_AuthRetrySeconds);
            while (true)
            {
                yield return wait;
                if (m_Auth.Status == FirebaseAuthStatus.SignedOut && Application.internetReachability != NetworkReachability.NotReachable)
                    m_Auth.TrySignIn("periodic retry, network=" + Application.internetReachability);
            }
        }

        // Coming back to the app is a good moment to retry (e.g. after airplane mode).
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && IsReady && m_Auth != null && m_Auth.Status == FirebaseAuthStatus.SignedOut)
                m_Auth.TrySignIn("app regained focus");
        }

        // A scene (re)load always ends any running match, so config activation may proceed again.
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            m_RemoteConfig.SetGameplayActive(false);
        }

        private void Start()
        {
            if (s_Instance == this)
                m_Analytics.LogGameSessionStart();
        }

        // =====================================================================
        // Consent
        // =====================================================================

        public static bool IsDataCollectionEnabled
        {
            get
            {
                try { return PlayerPrefs.GetInt(k_CollectionPrefKey, k_DefaultCollectionEnabled ? 1 : 0) == 1; }
                catch { return k_DefaultCollectionEnabled; }
            }
        }

        /// <summary>Enables/disables Analytics and Crashlytics collection and persists the choice.</summary>
        public static void SetDataCollectionEnabled(bool enabled)
        {
            try { PlayerPrefs.SetInt(k_CollectionPrefKey, enabled ? 1 : 0); PlayerPrefs.Save(); } catch { }
            if (s_Instance == null) return;
            s_Instance.m_Analytics.ApplyConsent(enabled);
            s_Instance.m_Crashlytics.ApplyConsent(enabled);
        }

        // =====================================================================
        // Gameplay-facing API (safe no-ops when Firebase is unavailable)
        // =====================================================================

        public static void ReportMatchStarted(int players, int humans, int bots, int roundsToWin, string arena)
        {
            if (s_Instance == null) return;
            s_Instance.m_RemoteConfig.SetGameplayActive(true);
            s_Instance.m_Crashlytics.SetKey(FirebaseEventRegistry.KeyArena, arena ?? "unknown");
            s_Instance.m_Analytics.LogMatchStart(players, humans, bots, roundsToWin, arena);
        }

        public static void ReportTankSelected(string tankName)
        {
            if (s_Instance == null) return;
            s_Instance.m_Analytics.LogTankSelected(tankName);
        }

        public static void ReportRoundCompleted(int roundNumber, string result)
        {
            if (s_Instance == null) return;
            s_Instance.m_Analytics.LogRoundComplete(roundNumber, result);
        }

        public static void ReportMatchCompleted(string result, int roundsPlayed, int humanRoundsWon, float durationSeconds)
        {
            if (s_Instance == null) return;
            s_Instance.m_Analytics.LogMatchComplete(result, roundsPlayed, humanRoundsWon, durationSeconds);
            s_Instance.m_RemoteConfig.SetGameplayActive(false);
        }

        public static void ReportPowerUpCollected(string powerUpType)
        {
            if (s_Instance == null) return;
            s_Instance.m_Analytics.LogPowerUpCollected(powerUpType);
        }

        /// <summary>Records the coarse gameplay state (e.g. "MainMenu", "Game") as a Crashlytics key.</summary>
        public static void SetGameState(string state, int roundNumber = 0)
        {
            if (s_Instance == null) return;
            s_Instance.m_Crashlytics.SetKey(FirebaseEventRegistry.KeyGameState, state);
            s_Instance.m_Crashlytics.SetKey(FirebaseEventRegistry.KeyRound, roundNumber.ToString());
        }

        /// <summary>Reports a caught exception as non-fatal. Caller keeps handling the exception as usual.</summary>
        public static void LogNonFatal(Exception exception, string context = null)
        {
            if (s_Instance == null) return;
            s_Instance.m_Crashlytics.LogNonFatal(exception, context);
        }

        /// <summary>Adds a breadcrumb line to the next Crashlytics report. No personal data.</summary>
        public static void LogBreadcrumb(string message)
        {
            if (s_Instance == null) return;
            s_Instance.m_Crashlytics.Log(message);
        }
    }
}
