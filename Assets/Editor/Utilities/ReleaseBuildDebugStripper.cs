using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tanks.Complete.EditorTools
{
    /// <summary>
    /// Keeps developer-only tooling out of tester/release builds without touching any scene asset.
    /// While a player build runs with Development Build OFF, every in-game debug console (IngameDebugConsole.DebugLogManager)
    /// is removed from the scene copy being built. The scene file on disk is unchanged, and Development builds
    /// and Play Mode keep the console.
    /// Matching is by type name so this script needs no reference to the plugin's assembly.
    /// </summary>
    public class ReleaseBuildDebugStripper : IProcessSceneWithReport
    {
        private const string k_DebugConsoleType = "IngameDebugConsole.DebugLogManager";

        // Runs late so other scene processors see the scene first.
        public int callbackOrder => 1000;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (!BuildPipeline.isBuildingPlayer) return;          // not a player build (e.g. entering Play Mode)
            if (EditorUserBuildSettings.development) return;       // dev builds keep the console

            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (behaviour == null || behaviour.GetType().FullName != k_DebugConsoleType) continue;

                    // The manager sits on the console's root object; destroy that whole object.
                    var target = behaviour.transform.root.gameObject;
                    Debug.Log("[ReleaseBuild] Removing '" + target.name + "' (debug console) from scene '" + scene.name + "' for this non-development build.");
                    Object.DestroyImmediate(target);
                    break; // children of the destroyed object are gone; re-scan the next root
                }
            }
        }
    }
}
