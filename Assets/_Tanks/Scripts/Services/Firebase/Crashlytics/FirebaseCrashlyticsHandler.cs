using System;
using System.Collections.Generic;
using Firebase.Crashlytics;

namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>
    /// Wraps Crashlytics. Native crashes and uncaught managed exceptions are captured by the SDK itself once
    /// Firebase is initialised (nothing is swallowed here). This handler adds non-fatal reporting, breadcrumbs and keys.
    /// Never pass secrets or personal data to these methods.
    /// </summary>
    public class FirebaseCrashlyticsHandler : FirebaseServiceBase
    {
        private readonly bool m_CollectionEnabled;
        // Keys set before Firebase is ready are replayed on activation (latest value per key).
        private readonly Dictionary<string, string> m_PendingKeys = new Dictionary<string, string>();

        protected override string ServiceName => "Crashlytics";
        internal override string LogCategory => FirebaseLogger.Crashlytics;

        public FirebaseCrashlyticsHandler(bool collectionEnabled)
        {
            m_CollectionEnabled = collectionEnabled;
        }

        protected override void OnActivate()
        {
            Crashlytics.IsCrashlyticsCollectionEnabled = m_CollectionEnabled;
            Crashlytics.SetCustomKey("app_version", UnityEngine.Application.version);
            FirebaseLogger.Info(FirebaseLogger.Crashlytics, "Collection enabled=" + m_CollectionEnabled + ", pending keys to replay=" + m_PendingKeys.Count);
            foreach (var kv in m_PendingKeys)
                Crashlytics.SetCustomKey(kv.Key, kv.Value);
            m_PendingKeys.Clear();
        }

        internal void ApplyConsent(bool enabled)
        {
            Guard(() => Crashlytics.IsCrashlyticsCollectionEnabled = enabled);
        }

        public void SetKey(string key, string value)
        {
            if (!IsAvailable) { m_PendingKeys[key] = value; return; }
            Guard(() => Crashlytics.SetCustomKey(key, value));
        }

        public void Log(string message) => Guard(() => Crashlytics.Log(message));

        /// <summary>Reports a caught exception as non-fatal. The caller still handles the exception normally.</summary>
        public void LogNonFatal(Exception exception, string context = null)
        {
            if (exception == null) return;
            Guard(() =>
            {
                if (!string.IsNullOrEmpty(context)) Crashlytics.Log(context);
                Crashlytics.LogException(exception);
            });
        }
    }
}
