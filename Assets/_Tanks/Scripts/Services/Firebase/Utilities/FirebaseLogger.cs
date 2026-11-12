using System;
using UnityEngine;
using Firebase;

namespace Tanks.Complete.FirebaseSystem
{
    /// <summary>
    /// Single place for Firebase logging. Every line is "[FIREBASE][CATEGORY] message" so one logcat filter
    /// (adb logcat -s Unity | findstr FIREBASE) shows the whole flow.
    /// Info lines exist only in the Editor and Development builds; Warning/Error are always emitted.
    /// Never pass tokens, API keys, passwords or credentials to these methods.
    /// </summary>
    internal static class FirebaseLogger
    {
        // Categories
        public const string Core = "CORE";
        public const string Init = "INIT";
        public const string Auth = "AUTH";
        public const string Firestore = "FIRESTORE";
        public const string Analytics = "ANALYTICS";
        public const string Crashlytics = "CRASHLYTICS";
        public const string RemoteConfig = "REMOTE_CONFIG";

        private static string Format(string category, string message) => "[FIREBASE][" + category + "] " + message;

        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Info(string category, string message) => Debug.Log(Format(category, message));

        public static void Warning(string category, string message) => Debug.LogWarning(Format(category, message));

        public static void Error(string category, string message) => Debug.LogError(Format(category, message));

        /// <summary>
        /// Readable one-line description of an exception: type, message and (for Firebase exceptions) the numeric error code.
        /// Task failures wrap the real error in an AggregateException, so the base exception is used.
        /// </summary>
        public static string Describe(Exception exception, bool includeStack = false)
        {
            if (exception == null) return "no exception (task cancelled or unknown)";
            var e = exception.GetBaseException();
            string text = e.GetType().Name + ": " + e.Message;
            if (e is FirebaseException fe) text += " (ErrorCode=" + fe.ErrorCode + ")";
            if (includeStack) text += "\n" + e.StackTrace;
            return text;
        }
    }
}
