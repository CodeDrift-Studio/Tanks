namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>
    /// Single source of truth for Analytics event and parameter names.
    /// Rules (Firebase limits): event/param names are snake_case, start with a letter, max 40 chars, no reserved
    /// prefixes (firebase_, google_, ga_). String values max 100 chars. Use only long, double or string values.
    /// To add an event: add its constants here, add one method to FirebaseAnalyticsHandler, one wrapper in FirebaseManager.
    /// "session_start" is auto-collected by the SDK, so our own event is named game_session_start.
    /// No personal data: no user names, emails, device ids or free text.
    /// </summary>
    public static class FirebaseEventRegistry
    {
        // Events
        public const string GameSessionStart = "game_session_start";
        public const string MatchStart = "match_start";
        public const string MatchComplete = "match_complete";
        public const string TankSelected = "tank_selected";
        public const string RoundComplete = "round_complete";
        public const string PowerUpCollected = "powerup_collected";

        // Parameters
        public const string ParamPlayerCount = "player_count";
        public const string ParamHumanCount = "human_count";
        public const string ParamBotCount = "bot_count";
        public const string ParamRoundsToWin = "rounds_to_win";
        public const string ParamArena = "arena";
        public const string ParamTank = "tank";
        public const string ParamRound = "round_number";
        public const string ParamResult = "result";          // see Result* values
        public const string ParamRoundsPlayed = "rounds_played";
        public const string ParamHumanRoundsWon = "human_rounds_won";
        public const string ParamDurationSec = "duration_sec";
        public const string ParamPowerUpType = "powerup_type";

        // Values for ParamResult. Defined from the human player's side (first release is one human vs AI).
        public const string ResultWin = "win";
        public const string ResultLoss = "loss";
        public const string ResultDraw = "draw";

        // Crashlytics custom keys
        public const string KeyGameState = "game_state";
        public const string KeyRound = "round_number";
        public const string KeyArena = "arena";
    }
}
