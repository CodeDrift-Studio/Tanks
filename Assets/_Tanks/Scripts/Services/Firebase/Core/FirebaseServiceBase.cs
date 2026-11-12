using System;

namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>
    /// Base for the per-product handlers (Analytics, Crashlytics, Remote Config, Auth, PlayerData).
    /// A handler only touches its SDK after <see cref="Activate"/> was called, which the
    /// FirebaseManager does only once FirebaseApp is verified usable.
    /// </summary>
    public abstract class FirebaseServiceBase
    {
        protected abstract string ServiceName { get; }

        /// <summary>Log category (see FirebaseLogger) used for this service's lines.</summary>
        internal virtual string LogCategory => FirebaseLogger.Core;

        /// <summary>
        /// True from the moment activation starts until it fails. It is intentionally already true while
        /// <see cref="OnActivate"/> runs, so OnActivate can use <see cref="Guard"/> (a previous version set it afterwards,
        /// which made Guard silently skip calls issued during activation — e.g. the first anonymous sign-in).
        /// </summary>
        public bool IsAvailable { get; private set; }

        /// <summary>Called once by the manager on the main thread when Firebase is ready.</summary>
        internal void Activate()
        {
            if (IsAvailable)
            {
                FirebaseLogger.Warning(LogCategory, $"{ServiceName}: duplicate activation ignored.");
                return;
            }

            FirebaseLogger.Info(LogCategory, $"{ServiceName}: activation started.");
            IsAvailable = true;
            try
            {
                OnActivate();
                FirebaseLogger.Info(LogCategory, $"{ServiceName}: activation completed.");
            }
            catch (Exception e)
            {
                // A broken service must not take the others (or the game) down.
                IsAvailable = false;
                FirebaseLogger.Error(LogCategory, $"{ServiceName}: activation FAILED. {FirebaseLogger.Describe(e, true)}");
            }
        }

        protected abstract void OnActivate();

        /// <summary>Runs an SDK call, converting any exception into a logged warning (Firebase must never break gameplay).</summary>
        protected void Guard(Action sdkCall)
        {
            if (!IsAvailable)
            {
                FirebaseLogger.Warning(LogCategory, $"{ServiceName}: call skipped because the service is not available.");
                return;
            }
            try { sdkCall(); }
            catch (Exception e) { FirebaseLogger.Warning(LogCategory, $"{ServiceName}: call failed. {FirebaseLogger.Describe(e, true)}"); }
        }
    }
}
