using System;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>
    /// Runs the asynchronous Firebase dependency check once. The result is delivered on the main thread.
    /// Offline is not an error here: the check only verifies Google Play services, not connectivity.
    /// </summary>
    internal static class FirebaseInitializer
    {
        public static void Run(Action onReady, Action<string> onFailed)
        {
            FirebaseLogger.Info(FirebaseLogger.Init, "Initialization started. platform=" + Application.platform
                + " package=" + Application.identifier + " version=" + Application.version);
            try
            {
                FirebaseLogger.Info(FirebaseLogger.Init, "Dependency check started.");
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        onFailed("Dependency check did not complete: " + FirebaseLogger.Describe(task.Exception, true));
                        return;
                    }

                    FirebaseLogger.Info(FirebaseLogger.Init, "Dependency check completed. status=" + task.Result);
                    if (task.Result != DependencyStatus.Available)
                    {
                        onFailed("Dependencies unavailable: " + task.Result);
                        return;
                    }
                    try
                    {
                        // Touching DefaultInstance creates the app from google-services.json.
                        var app = FirebaseApp.DefaultInstance;
                        if (app == null)
                        {
                            onFailed("FirebaseApp.DefaultInstance is null");
                            return;
                        }

                        // Project/app identifiers are not secrets. Compare them with the Firebase Console to rule out
                        // "looking at the wrong project". The API key is deliberately NOT logged.
                        var o = app.Options;
                        FirebaseLogger.Info(FirebaseLogger.Init, "FirebaseApp available. name=" + app.Name
                            + " projectId=" + o.ProjectId + " appId=" + o.AppId);
                    }
                    catch (Exception e)
                    {
                        onFailed("FirebaseApp creation failed: " + FirebaseLogger.Describe(e, true));
                        return;
                    }
                    onReady();
                });
            }
            catch (Exception e)
            {
                onFailed("Dependency check threw: " + FirebaseLogger.Describe(e, true));
            }
        }
    }
}
