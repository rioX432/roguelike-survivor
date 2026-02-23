using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using RoguelikeSurvivor;

/// <summary>
/// Automatically adds GameBootstrap to the scene before each build if not present.
/// Also runnable manually via GlitchClaw/Inject Bootstrap.
/// </summary>
public class SceneBootstrapInjector : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Open the main scene and ensure GameBootstrap is present
        InjectBootstrapIntoScene("Assets/Scenes/MainGame.unity");
    }

    [MenuItem("GlitchClaw/Inject Bootstrap Into Scene")]
    public static void InjectBootstrapMenuItem()
    {
        InjectBootstrapIntoScene("Assets/Scenes/MainGame.unity");
    }

    private static void InjectBootstrapIntoScene(string scenePath)
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
            scenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);

        // Check if GameBootstrap already exists
        var existing = Object.FindFirstObjectByType<GameBootstrap>();
        if (existing != null)
        {
            Debug.Log("[SceneBootstrapInjector] GameBootstrap already present — skipping.");
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            return;
        }

        // Find Main Camera or any existing GameObject to attach to
        var camGO = GameObject.Find("Main Camera");
        if (camGO == null)
        {
            // Create a dedicated bootstrap object
            camGO = new GameObject("Bootstrap");
            Debug.Log("[SceneBootstrapInjector] Created new Bootstrap GameObject.");
        }

        camGO.AddComponent<GameBootstrap>();
        Debug.Log($"[SceneBootstrapInjector] Added GameBootstrap to '{camGO.name}'.");

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        Debug.Log("[SceneBootstrapInjector] Scene saved.");
    }
}
