using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.RemoteConfig;

namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>
    /// Remote Config is optional: the game always works from the in-code defaults, then from the last cached/activated values.
    /// Fetched values are only ACTIVATED while no match is running, so a match never changes rules mid-play.
    ///
    /// Adding a parameter: 1) add a constant in Keys, 2) add it (with a default) in BuildDefaults(),
    /// 3) read it with GetInt/GetFloat/GetBool/GetString. Accessors return the default if Remote Config is unavailable.
    /// The parameters below are NOT yet read by gameplay code (no gameplay behaviour was changed).
    /// </summary>
    public class FirebaseRemoteConfigHandler : FirebaseServiceBase
    {
        public static class Keys
        {
            public const string ConfigVersion = "config_version";
            public const string MatchRoundsToWin = "match_rounds_to_win";
            public const string PowerUpRespawnSeconds = "powerup_respawn_seconds";
        }

        private static Dictionary<string, object> BuildDefaults() => new Dictionary<string, object>
        {
            { Keys.ConfigVersion, "local-default" },
            { Keys.MatchRoundsToWin, 3L },          // mirrors Desert's scene value (GameManager.m_NumRoundsToWin = 3); not read by gameplay yet
            { Keys.PowerUpRespawnSeconds, 20.0 },   // mirrors the PowerUpSpawner cooldown
        };

        private readonly Dictionary<string, object> m_Defaults = BuildDefaults();
        private bool m_FetchedPendingActivation;
        private bool m_GameplayActive;

        protected override string ServiceName => "RemoteConfig";
        internal override string LogCategory => FirebaseLogger.RemoteConfig;

        /// <summary>Raised on the main thread after new values were activated.</summary>
        public event Action ValuesActivated;

        protected override void OnActivate()
        {
            FirebaseLogger.Info(FirebaseLogger.RemoteConfig, "Applying settings/defaults and starting fetch.");
            var rc = FirebaseRemoteConfig.DefaultInstance;
            // Zero interval in dev so changes are testable; 12h in production to respect fetch throttling.
            bool dev = UnityEngine.Debug.isDebugBuild || UnityEngine.Application.isEditor;
            var settings = new ConfigSettings
            {
                MinimumFetchIntervalInMilliseconds = dev ? 0UL : 12UL * 3600UL * 1000UL,
                FetchTimeoutInMilliseconds = 10000UL
            };
            rc.SetConfigSettingsAsync(settings).ContinueWithOnMainThread(settingsTask =>
            {
                // Defaults are applied even if settings failed; fetch then proceeds regardless.
                rc.SetDefaultsAsync(m_Defaults).ContinueWithOnMainThread(defaultsTask => { Fetch(); });
            });
        }

        /// <summary>Called by the manager when a match starts/ends so activation never happens mid-match.</summary>
        internal void SetGameplayActive(bool active)
        {
            m_GameplayActive = active;
            if (!active && m_FetchedPendingActivation) ActivateFetched();
        }

        private void Fetch()
        {
            Guard(() =>
                FirebaseRemoteConfig.DefaultInstance.FetchAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        // Offline / throttled / backend error: keep cached or default values.
                        FirebaseLogger.Warning(FirebaseLogger.RemoteConfig, "Fetch failed; using cached/default values. " + FirebaseLogger.Describe(task.Exception));
                        return;
                    }
                    FirebaseLogger.Info(FirebaseLogger.RemoteConfig, "Fetch succeeded. gameplayActive=" + m_GameplayActive);
                    m_FetchedPendingActivation = true;
                    if (!m_GameplayActive) ActivateFetched();
                }));
        }

        private void ActivateFetched()
        {
            m_FetchedPendingActivation = false;
            Guard(() =>
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled) return;
                    FirebaseLogger.Info(FirebaseLogger.RemoteConfig, "Values activated.");
                    ValuesActivated?.Invoke();
                }));
        }

        // ---------- Typed accessors (safe before/without Firebase: return the registered default) ----------

        public int GetInt(string key) => (int)GetLong(key);

        public long GetLong(string key)
        {
            if (IsAvailable)
            {
                try { return FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue; }
                catch (Exception e) { FirebaseLogger.Warning(FirebaseLogger.RemoteConfig, "Read failed. " + FirebaseLogger.Describe(e)); }
            }
            return m_Defaults.TryGetValue(key, out var d) ? Convert.ToInt64(d) : 0L;
        }

        public float GetFloat(string key)
        {
            if (IsAvailable)
            {
                try { return (float)FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue; }
                catch (Exception e) { FirebaseLogger.Warning(FirebaseLogger.RemoteConfig, "Read failed. " + FirebaseLogger.Describe(e)); }
            }
            return m_Defaults.TryGetValue(key, out var d) ? Convert.ToSingle(d) : 0f;
        }

        public bool GetBool(string key)
        {
            if (IsAvailable)
            {
                try { return FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue; }
                catch (Exception e) { FirebaseLogger.Warning(FirebaseLogger.RemoteConfig, "Read failed. " + FirebaseLogger.Describe(e)); }
            }
            return m_Defaults.TryGetValue(key, out var d) && Convert.ToBoolean(d);
        }

        public string GetString(string key)
        {
            if (IsAvailable)
            {
                try { return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue; }
                catch (Exception e) { FirebaseLogger.Warning(FirebaseLogger.RemoteConfig, "Read failed. " + FirebaseLogger.Describe(e)); }
            }
            return m_Defaults.TryGetValue(key, out var d) ? Convert.ToString(d) : string.Empty;
        }
    }
}
