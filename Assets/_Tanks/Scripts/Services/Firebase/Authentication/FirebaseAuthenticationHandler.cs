using System;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

namespace Tanks.Complete.FirebaseSystem
{
    public enum FirebaseAuthStatus
    {
        NotStarted,
        /// <summary>Waiting for an anonymous sign-in to finish.</summary>
        SigningIn,
        /// <summary>Firebase returned a valid user (UID available).</summary>
        SignedIn,
        /// <summary>Not signed in (offline or error). Retried by the manager; the game is unaffected.</summary>
        SignedOut,
        /// <summary>Firebase initialization failed, so authentication cannot run this session.</summary>
        Unavailable
    }

    public enum AuthLinkResult
    {
        Linked,
        /// <summary>The credential already belongs to another Firebase account. Nothing was changed.</summary>
        CredentialAlreadyInUse,
        NotSignedIn,
        Failed
    }

    /// <summary>
    /// Anonymous authentication lifecycle.
    /// - Firebase Auth persists its session on the device itself, so a returning player already has CurrentUser:
    ///   we reuse it and NEVER call SignInAnonymously in that case (the UID stays stable).
    /// - A new anonymous account is only created when there is no CurrentUser and no sign-in is already running.
    /// - SignedIn is only reported when CurrentUser is non-null after Firebase returned.
    /// - Failures (offline etc.) leave status SignedOut; the manager calls <see cref="TrySignIn"/> again later.
    /// - <see cref="LinkCredential"/> upgrades the anonymous account to a real credential (e.g. Google Play Games, added
    ///   later) while keeping the same UID, so progress stored under players/{uid} stays valid. No GPGS code lives here.
    /// The UID is a pseudonymous identifier: it is logged for diagnostics only (Editor/Development builds) and must never be
    /// attached to Analytics or Crashlytics. Tokens/credentials are never logged.
    /// </summary>
    public class FirebaseAuthenticationHandler : FirebaseServiceBase
    {
        private const string Tag = FirebaseLogger.Auth;

        private FirebaseAuth m_Auth;
        private bool m_SignInInFlight;
        private bool m_Linking;
        private int m_SignInAttempts;
        private string m_LastError;

        protected override string ServiceName => "Authentication";
        internal override string LogCategory => Tag;

        public FirebaseAuthStatus Status { get; private set; } = FirebaseAuthStatus.NotStarted;

        /// <summary>Current Firebase UID, or null when signed out.</summary>
        public string UserId => IsAvailable && m_Auth != null && m_Auth.CurrentUser != null ? m_Auth.CurrentUser.UserId : null;

        /// <summary>True when the account is still anonymous (not yet linked to another provider).</summary>
        public bool IsAnonymous => IsAvailable && m_Auth != null && m_Auth.CurrentUser != null && m_Auth.CurrentUser.IsAnonymous;

        /// <summary>Number of anonymous sign-in requests sent this session (diagnostics; should normally be 0 or 1).</summary>
        public int SignInAttempts => m_SignInAttempts;
        public string LastError => m_LastError;

        /// <summary>Raised on the main thread when sign-in state changes. Argument: new UID or null.</summary>
        public event Action<FirebaseAuthStatus, string> StatusChanged;

        public FirebaseAuthenticationHandler()
        {
            FirebaseLogger.Info(Tag, "Authentication handler created.");
        }

        /// <summary>Marks auth unusable because Firebase itself failed to initialize.</summary>
        internal void MarkUnavailable()
        {
            SetStatus(FirebaseAuthStatus.Unavailable, "Firebase initialization failed");
        }

        protected override void OnActivate()
        {
            FirebaseLogger.Info(Tag, "Authentication initialization started.");
            m_Auth = FirebaseAuth.DefaultInstance;
            if (m_Auth == null) throw new InvalidOperationException("FirebaseAuth.DefaultInstance returned null");
            FirebaseLogger.Info(Tag, "FirebaseAuth instance acquired.");

            m_Auth.StateChanged += OnAuthStateChanged;
            Refresh("initial activation");
        }

        /// <summary>Safe to call repeatedly (retry on connectivity return / app focus). Does nothing if already signed in or busy.</summary>
        public void TrySignIn(string reason)
        {
            if (!IsAvailable)
            {
                FirebaseLogger.Warning(Tag, "Retry requested (" + reason + ") but authentication is not available.");
                return;
            }
            FirebaseLogger.Info(Tag, "Retry triggered. reason=" + reason + " status=" + Status);
            Refresh(reason);
        }

        private void Refresh(string reason)
        {
            var existing = m_Auth.CurrentUser;
            if (existing != null)
            {
                FirebaseLogger.Info(Tag, "Existing user session found; NOT creating a new anonymous user. " + DescribeUser(existing));
                SetStatus(FirebaseAuthStatus.SignedIn, "existing session restored");
                return;
            }

            FirebaseLogger.Info(Tag, "Current user is null (" + reason + ").");
            if (m_SignInInFlight)
            {
                FirebaseLogger.Info(Tag, "Sign-in already in flight; not sending another request.");
                return;
            }

            m_SignInInFlight = true;
            m_SignInAttempts++;
            SetStatus(FirebaseAuthStatus.SigningIn, "anonymous sign-in requested");
            FirebaseLogger.Info(Tag, "Anonymous sign-in requested. attempt=" + m_SignInAttempts);

            try
            {
                m_Auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
                {
                    m_SignInInFlight = false;
                    FirebaseLogger.Info(Tag, "Sign-in request completed. faulted=" + task.IsFaulted + " canceled=" + task.IsCanceled);

                    if (task.IsFaulted || task.IsCanceled)
                    {
                        m_LastError = DescribeAuthError(task.Exception);
                        FirebaseLogger.Warning(Tag, "Anonymous sign-in FAILED (will retry while signed out). " + m_LastError);
                        SetStatus(FirebaseAuthStatus.SignedOut, "sign-in failed");
                        return;
                    }

                    // Do not trust the task alone: only report SignedIn when Firebase actually holds a user.
                    var user = m_Auth.CurrentUser;
                    if (user == null)
                    {
                        m_LastError = "sign-in task succeeded but CurrentUser is null";
                        FirebaseLogger.Error(Tag, m_LastError);
                        SetStatus(FirebaseAuthStatus.SignedOut, "no user after sign-in");
                        return;
                    }

                    m_LastError = null;
                    FirebaseLogger.Info(Tag, "Anonymous sign-in SUCCEEDED (new user). " + DescribeUser(user));
                    SetStatus(FirebaseAuthStatus.SignedIn, "anonymous sign-in succeeded");
                });
            }
            catch (Exception e)
            {
                // Synchronous failure: without this the in-flight flag would stay set and block every retry.
                m_SignInInFlight = false;
                m_LastError = FirebaseLogger.Describe(e);
                FirebaseLogger.Error(Tag, "Sign-in request threw before starting. " + FirebaseLogger.Describe(e, true));
                SetStatus(FirebaseAuthStatus.SignedOut, "sign-in threw");
            }
        }

        // Covers sign-out or token invalidation that happens outside our own calls.
        private void OnAuthStateChanged(object sender, EventArgs args)
        {
            var user = m_Auth.CurrentUser;
            FirebaseLogger.Info(Tag, "Authentication state changed. hasUser=" + (user != null) + " signInInFlight=" + m_SignInInFlight);

            if (user != null) SetStatus(FirebaseAuthStatus.SignedIn, "state changed: user present");
            else SetStatus(m_SignInInFlight ? FirebaseAuthStatus.SigningIn : FirebaseAuthStatus.SignedOut, "state changed: no user");
        }

        /// <summary>
        /// Links the current anonymous account to a credential obtained elsewhere (future GPGS flow). The UID is unchanged.
        /// If the credential belongs to an existing account we do NOT switch accounts here; the caller decides what to do.
        /// </summary>
        public void LinkCredential(Credential credential, Action<AuthLinkResult> onDone)
        {
            var user = IsAvailable ? m_Auth.CurrentUser : null;
            if (user == null) { onDone?.Invoke(AuthLinkResult.NotSignedIn); return; }
            if (m_Linking) { onDone?.Invoke(AuthLinkResult.Failed); return; }

            m_Linking = true;
            FirebaseLogger.Info(Tag, "Credential link requested (uid unchanged on success).");
            try
            {
                user.LinkWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                {
                    m_Linking = false;
                    if (!task.IsFaulted && !task.IsCanceled)
                    {
                        FirebaseLogger.Info(Tag, "Credential link succeeded. " + DescribeUser(m_Auth.CurrentUser));
                        onDone?.Invoke(AuthLinkResult.Linked);
                        return;
                    }
                    var fe = task.Exception?.GetBaseException() as FirebaseException;
                    bool inUse = fe != null && fe.ErrorCode == (int)AuthError.CredentialAlreadyInUse;
                    FirebaseLogger.Warning(Tag, "Credential link failed. " + DescribeAuthError(task.Exception));
                    onDone?.Invoke(inUse ? AuthLinkResult.CredentialAlreadyInUse : AuthLinkResult.Failed);
                });
            }
            catch (Exception e)
            {
                m_Linking = false;
                FirebaseLogger.Warning(Tag, "Credential link threw. " + FirebaseLogger.Describe(e, true));
                onDone?.Invoke(AuthLinkResult.Failed);
            }
        }

        private void SetStatus(FirebaseAuthStatus status, string why)
        {
            if (Status == status) return;
            var old = Status;
            Status = status;
            FirebaseLogger.Info(Tag, "Auth status " + old + " -> " + status + " (" + why + ")");
            StatusChanged?.Invoke(status, UserId);
        }

        /// <summary>One-line state summary for the manager's diagnostic snapshot.</summary>
        internal string DescribeState()
        {
            return "status=" + Status + " available=" + IsAvailable + " signInAttempts=" + m_SignInAttempts
                + " inFlight=" + m_SignInInFlight + " hasUser=" + (UserId != null)
                + (m_LastError != null ? " lastError=[" + m_LastError + "]" : "");
        }

        // UID is logged on purpose (diagnostics only, dev builds) so it can be matched against Console > Authentication > Users.
        private static string DescribeUser(FirebaseUser user)
        {
            if (user == null) return "user=null";
            string created = "?", lastSignIn = "?";
            try
            {
                var m = user.Metadata;
                created = DateTimeOffset.FromUnixTimeMilliseconds((long)m.CreationTimestamp).UtcDateTime.ToString("u");
                lastSignIn = DateTimeOffset.FromUnixTimeMilliseconds((long)m.LastSignInTimestamp).UtcDateTime.ToString("u");
            }
            catch { /* metadata is a nice-to-have */ }
            return "uid=" + user.UserId + " isAnonymous=" + user.IsAnonymous + " created=" + created + " lastSignIn=" + lastSignIn;
        }

        // Adds the readable AuthError name to the numeric error code (e.g. OperationNotAllowed = Anonymous provider disabled).
        private static string DescribeAuthError(Exception exception)
        {
            string text = FirebaseLogger.Describe(exception);
            if (exception?.GetBaseException() is FirebaseException fe)
                text += " AuthError=" + (AuthError)fe.ErrorCode;
            return text;
        }
    }
}
