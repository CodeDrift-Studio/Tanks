using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;

namespace Tanks.Complete.FirebaseSystem
{
    public enum PlayerDataStatus
    {
        Success,
        /// <summary>The player document does not exist yet (reads only).</summary>
        NotFound,
        NotSignedIn,
        /// <summary>Firebase/Firestore is unavailable this session.</summary>
        Unavailable,
        Failed
    }

    public readonly struct PlayerDataReadResult
    {
        public readonly PlayerDataStatus Status;
        /// <summary>Raw fields of players/{uid}; null unless Status is Success.</summary>
        public readonly IDictionary<string, object> Fields;

        public PlayerDataReadResult(PlayerDataStatus status, IDictionary<string, object> fields = null)
        {
            Status = status;
            Fields = fields;
        }
    }

    /// <summary>
    /// Low-level access to the signed-in player's Firestore document, players/{uid}.
    ///
    /// DELIBERATELY SCHEMA-FREE: this class knows nothing about which fields exist. It only reads and merges raw
    /// dictionaries, so real player data can be added later without restructuring the Firebase system.
    /// It does NOT sync automatically and nothing in the game calls it yet. Orchestration (when to read/write,
    /// conflict policy, retry on connectivity) belongs to a future FirebaseCloudSyncHandler, once the game has
    /// real player data to persist (see Documentation/Development/FIREBASE_AUTH_AND_CLOUD_SYNC.md).
    ///
    /// Authentication stays separate: the UID comes from the auth handler through the supplied delegate. The document
    /// path uses the Firebase UID only, so a later GPGS credential link (same UID) does not create a second record.
    ///
    /// Offline behaviour (Firestore SDK): reads are served from the local cache when possible; writes are queued by the
    /// SDK and their completion callback fires only after the server acknowledges them, which can be much later.
    /// </summary>
    public class FirebasePlayerDataHandler : FirebaseServiceBase
    {
        public const string CollectionName = "players";

        private readonly Func<string> m_UserIdProvider;
        private FirebaseFirestore m_Firestore;

        protected override string ServiceName => "PlayerData";
        internal override string LogCategory => FirebaseLogger.Firestore;

        public FirebasePlayerDataHandler(Func<string> userIdProvider)
        {
            m_UserIdProvider = userIdProvider;
        }

        protected override void OnActivate()
        {
            // Offline persistence is on by default on Android; settings are intentionally left untouched.
            m_Firestore = FirebaseFirestore.DefaultInstance;
            FirebaseLogger.Info(FirebaseLogger.Firestore, "Firestore instance acquired. No automatic reads/writes are performed.");
        }

        /// <summary>Reads players/{uid}. The callback always runs on the main thread, exactly once.</summary>
        public void ReadPlayerDocument(Action<PlayerDataReadResult> onDone)
        {
            FirebaseLogger.Info(FirebaseLogger.Firestore, "Read requested. " + AuthSummary());
            if (!TryGetDocument(out var doc, out var failure))
            {
                onDone?.Invoke(new PlayerDataReadResult(failure));
                return;
            }

            try
            {
                doc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        FirebaseLogger.Warning(FirebaseLogger.Firestore, "Read FAILED. " + FirebaseLogger.Describe(task.Exception));
                        onDone?.Invoke(new PlayerDataReadResult(PlayerDataStatus.Failed));
                        return;
                    }
                    var snapshot = task.Result;
                    FirebaseLogger.Info(FirebaseLogger.Firestore, "Read succeeded. exists=" + snapshot.Exists);
                    onDone?.Invoke(snapshot.Exists
                        ? new PlayerDataReadResult(PlayerDataStatus.Success, snapshot.ToDictionary())
                        : new PlayerDataReadResult(PlayerDataStatus.NotFound));
                });
            }
            catch (Exception e)
            {
                FirebaseLogger.Warning(FirebaseLogger.Firestore, "Read threw. " + FirebaseLogger.Describe(e, true));
                onDone?.Invoke(new PlayerDataReadResult(PlayerDataStatus.Failed));
            }
        }

        /// <summary>
        /// Merges the given fields into players/{uid} (creating it if missing); fields not mentioned are left untouched.
        /// The callback runs on the main thread once the server acknowledges the write (may be delayed while offline).
        /// </summary>
        public void MergePlayerDocument(IDictionary<string, object> fields, Action<PlayerDataStatus> onDone)
        {
            if (fields == null || fields.Count == 0)
            {
                onDone?.Invoke(PlayerDataStatus.Success); // nothing to write; avoids pointless writes
                return;
            }
            FirebaseLogger.Info(FirebaseLogger.Firestore, "Merge requested. fieldCount=" + fields.Count + ". " + AuthSummary());
            if (!TryGetDocument(out var doc, out var failure))
            {
                onDone?.Invoke(failure);
                return;
            }

            try
            {
                doc.SetAsync(fields, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        FirebaseLogger.Warning(FirebaseLogger.Firestore, "Merge FAILED. " + FirebaseLogger.Describe(task.Exception));
                        onDone?.Invoke(PlayerDataStatus.Failed);
                        return;
                    }
                    FirebaseLogger.Info(FirebaseLogger.Firestore, "Merge acknowledged by server.");
                    onDone?.Invoke(PlayerDataStatus.Success);
                });
            }
            catch (Exception e)
            {
                FirebaseLogger.Warning(FirebaseLogger.Firestore, "Merge threw. " + FirebaseLogger.Describe(e, true));
                onDone?.Invoke(PlayerDataStatus.Failed);
            }
        }

        private string AuthSummary()
        {
            string uid = m_UserIdProvider?.Invoke();
            return "uidAvailable=" + !string.IsNullOrEmpty(uid);
        }

        private bool TryGetDocument(out DocumentReference doc, out PlayerDataStatus failure)
        {
            doc = null;
            if (!IsAvailable)
            {
                FirebaseLogger.Warning(FirebaseLogger.Firestore, "Request rejected: Firestore service unavailable.");
                failure = PlayerDataStatus.Unavailable;
                return false;
            }

            string uid = m_UserIdProvider?.Invoke();
            if (string.IsNullOrEmpty(uid))
            {
                FirebaseLogger.Warning(FirebaseLogger.Firestore, "Request rejected: no authenticated UID yet.");
                failure = PlayerDataStatus.NotSignedIn;
                return false;
            }

            try
            {
                doc = m_Firestore.Collection(CollectionName).Document(uid);
                FirebaseLogger.Info(FirebaseLogger.Firestore, "Document path=" + CollectionName + "/" + uid);
                failure = PlayerDataStatus.Success;
                return true;
            }
            catch (Exception e)
            {
                FirebaseLogger.Warning(FirebaseLogger.Firestore, "Could not create document reference. " + FirebaseLogger.Describe(e, true));
                failure = PlayerDataStatus.Failed;
                return false;
            }
        }
    }
}
