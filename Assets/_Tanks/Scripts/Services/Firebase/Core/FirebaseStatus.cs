namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>Lifecycle of the Firebase SDK. Gameplay never waits on this.</summary>
    public enum FirebaseStatus
    {
        NotStarted,
        Initializing,
        Ready,
        /// <summary>Dependency check or initialization failed. Firebase calls become no-ops for this session.</summary>
        Failed
    }
}
