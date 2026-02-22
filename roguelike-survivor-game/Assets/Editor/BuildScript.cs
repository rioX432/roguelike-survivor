using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

public static class BuildScript
{
    private static readonly string[] GameScenes = GetEnabledScenes();

    private static string[] GetEnabledScenes()
    {
        return EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();
    }

    [MenuItem("Build/Android APK")]
    public static void BuildAndroid()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GameScenes,
            locationPathName = "../build/android/GlitchClaw.apk",
            target = BuildTarget.Android,
            options = BuildOptions.CompressWithLz4HC
        };

        PlayerSettings.SetApplicationIdentifier(
            BuildTargetGroup.Android, "com.glitchclaw.survivor");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"Android build failed: {report.summary.totalErrors} error(s)");
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log($"Android build succeeded: {report.summary.outputPath}");
        }
    }

    [MenuItem("Build/iOS Xcode Project")]
    public static void BuildiOS()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GameScenes,
            locationPathName = "../build/ios",
            target = BuildTarget.iOS,
            options = BuildOptions.CompressWithLz4HC
        };

        PlayerSettings.SetApplicationIdentifier(
            BuildTargetGroup.iOS, "com.glitchclaw.survivor");

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"iOS build failed: {report.summary.totalErrors} error(s)");
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log($"iOS build succeeded: {report.summary.outputPath}");
        }
    }
}
