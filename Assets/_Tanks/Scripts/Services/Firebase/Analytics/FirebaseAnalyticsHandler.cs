using System.Collections.Generic;
using Firebase.Analytics;

namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>
    /// Wraps FirebaseAnalytics. Events requested before Firebase is ready are buffered (bounded) and sent on activation;
    /// if Firebase fails they are dropped. Gameplay code never calls this directly: use FirebaseManager.
    /// </summary>
    public class FirebaseAnalyticsHandler : FirebaseServiceBase
    {
        private const int k_MaxBuffered = 32;
        private readonly Queue<(string name, Parameter[] parameters)> m_Buffer = new Queue<(string, Parameter[])>();
        private readonly bool m_CollectionEnabled;
        private bool m_Failed;

        protected override string ServiceName => "Analytics";
        internal override string LogCategory => FirebaseLogger.Analytics;

        public FirebaseAnalyticsHandler(bool collectionEnabled)
        {
            m_CollectionEnabled = collectionEnabled;
        }

        protected override void OnActivate()
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(m_CollectionEnabled);
            FirebaseLogger.Info(FirebaseLogger.Analytics, "Collection enabled=" + m_CollectionEnabled + ", buffered events=" + m_Buffer.Count + " (flushed right after activation).");
        }

        internal void ApplyConsent(bool enabled)
        {
            Guard(() => FirebaseAnalytics.SetAnalyticsCollectionEnabled(enabled));
        }

        internal void MarkFailed()
        {
            m_Failed = true;
            m_Buffer.Clear();
        }

        internal void FlushBuffer()
        {
            while (m_Buffer.Count > 0)
            {
                var e = m_Buffer.Dequeue();
                Send(e.name, e.parameters);
            }
        }

        // ---------- Event API (called through FirebaseManager) ----------

        public void LogGameSessionStart() => Log(FirebaseEventRegistry.GameSessionStart);

        public void LogMatchStart(int players, int humans, int bots, int roundsToWin, string arena) =>
            Log(FirebaseEventRegistry.MatchStart,
                new Parameter(FirebaseEventRegistry.ParamPlayerCount, players),
                new Parameter(FirebaseEventRegistry.ParamHumanCount, humans),
                new Parameter(FirebaseEventRegistry.ParamBotCount, bots),
                new Parameter(FirebaseEventRegistry.ParamRoundsToWin, roundsToWin),
                new Parameter(FirebaseEventRegistry.ParamArena, Trim(arena)));

        public void LogTankSelected(string tankName) =>
            Log(FirebaseEventRegistry.TankSelected, new Parameter(FirebaseEventRegistry.ParamTank, Trim(tankName)));

        public void LogRoundComplete(int roundNumber, string result) =>
            Log(FirebaseEventRegistry.RoundComplete,
                new Parameter(FirebaseEventRegistry.ParamRound, roundNumber),
                new Parameter(FirebaseEventRegistry.ParamResult, result));

        public void LogMatchComplete(string result, int roundsPlayed, int humanRoundsWon, double durationSeconds) =>
            Log(FirebaseEventRegistry.MatchComplete,
                new Parameter(FirebaseEventRegistry.ParamResult, result),
                new Parameter(FirebaseEventRegistry.ParamRoundsPlayed, roundsPlayed),
                new Parameter(FirebaseEventRegistry.ParamHumanRoundsWon, humanRoundsWon),
                new Parameter(FirebaseEventRegistry.ParamDurationSec, System.Math.Round(durationSeconds, 1)));

        public void LogPowerUpCollected(string powerUpType) =>
            Log(FirebaseEventRegistry.PowerUpCollected, new Parameter(FirebaseEventRegistry.ParamPowerUpType, Trim(powerUpType)));

        // ---------- Internals ----------

        private void Log(string name, params Parameter[] parameters)
        {
            if (m_Failed) return;

            if (!IsAvailable)
            {
                if (m_Buffer.Count < k_MaxBuffered)
                    m_Buffer.Enqueue((name, parameters));
                return;
            }
            Send(name, parameters);
        }

        private void Send(string name, Parameter[] parameters)
        {
            Guard(() => FirebaseAnalytics.LogEvent(name, parameters));
        }

        private static string Trim(string s)
        {
            if (string.IsNullOrEmpty(s)) return "unknown";
            return s.Length > 100 ? s.Substring(0, 100) : s;
        }
    }
}
