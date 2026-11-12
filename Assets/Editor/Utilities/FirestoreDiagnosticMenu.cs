using Tanks.Complete.FirebaseSystem;
using UnityEditor;
using UnityEngine;

namespace Tanks.Complete.EditorTools
{
    /// <summary>Manual trigger for the Firestore connectivity diagnostic. Never runs automatically.</summary>
    public static class FirestoreDiagnosticMenu
    {
        private const string k_Path = "Tools/Firebase/Run Firestore Diagnostic";

        [MenuItem(k_Path)]
        private static void RunDiagnostic()
        {
            // Creates/uses a real anonymous account and writes then deletes one document in the live Firebase project.
            FirebaseFirestoreDiagnostic.Run();
        }

        [MenuItem(k_Path, true)]
        private static bool Validate() => Application.isPlaying;
    }
}
