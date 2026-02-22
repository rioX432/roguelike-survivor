#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RoguelikeSurvivor.Editor
{
    public static class BuildScript
    {
        private static readonly string[] Scenes = { "Assets/Scenes/MainGame.unity" };

        public static void BuildAndroid()
        {
            var options = new BuildPlayerOptions
            {
                scenes = Scenes,
                locationPathName = "Build/Android/RoguelikeSurvivor.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"Android build failed: {report.summary.totalErrors} errors");
                EditorApplication.Exit(1);
            }
        }

        public static void BuildiOS()
        {
            var options = new BuildPlayerOptions
            {
                scenes = Scenes,
                locationPathName = "Build/iOS",
                target = BuildTarget.iOS,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"iOS build failed: {report.summary.totalErrors} errors");
                EditorApplication.Exit(1);
            }
        }
    }
}
#endif
