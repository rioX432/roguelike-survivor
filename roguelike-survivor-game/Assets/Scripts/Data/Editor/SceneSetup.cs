#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Sets up the MainGame scene with all required GameObjects and components.
    /// Run from GlitchClaw/Setup MainGame Scene menu.
    /// </summary>
    public static class SceneSetup
    {
        [MenuItem("GlitchClaw/Setup MainGame Scene")]
        public static void SetupScene()
        {
            // --- Core Managers ---
            CreateManager<GameManager>("GameManager");
            CreateManager<PoolManager>("PoolManager");

            var audioGO = CreateManager<AudioManager>("AudioManager");
            // Add 6 SE AudioSources
            var bgmSource = audioGO.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            for (int i = 0; i < 6; i++)
            {
                var se = audioGO.AddComponent<AudioSource>();
                se.playOnAwake = false;
            }
            var am = audioGO.GetComponent<AudioManager>();
            var serialized = new SerializedObject(am);
            serialized.FindProperty("_bgmSource").objectReferenceValue = bgmSource;
            // SE pool via reflection-like serialized field
            var sePoolProp = serialized.FindProperty("_sePool");
            sePoolProp.arraySize = 6;
            var sources = audioGO.GetComponents<AudioSource>();
            for (int i = 0; i < 6 && i + 1 < sources.Length; i++)
                sePoolProp.GetArrayElementAtIndex(i).objectReferenceValue = sources[i + 1];
            // Auto-assign BGM clip
            var bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/Crimson_banners_of_the_Sunken_Isles.mp3");
            if (bgmClip != null)
                serialized.FindProperty("_bgmClip").objectReferenceValue = bgmClip;
            serialized.ApplyModifiedProperties();

            var levelUpGO = CreateManager<LevelUpSystem>("LevelUpSystem");

            // --- Camera ---
            var cameraGO = GameObject.Find("Main Camera") ?? new GameObject("Main Camera");
            if (cameraGO.GetComponent<Camera>() == null)
                cameraGO.AddComponent<Camera>().orthographic = true;
            var cam = cameraGO.GetComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.12f);
            cameraGO.transform.position = new Vector3(0, 0, -10);
            if (cameraGO.GetComponent<CameraFollow>() == null)
                cameraGO.AddComponent<CameraFollow>();

            // --- Infinite Map (3x3 chunks) ---
            var mapGO = new GameObject("InfiniteMap");
            var mapCtrl = mapGO.AddComponent<InfiniteMapController>();
            var chunks = new Transform[9];
            for (int i = 0; i < 9; i++)
            {
                var chunk = new GameObject($"Chunk_{i}");
                chunk.transform.SetParent(mapGO.transform);
                var sr = chunk.AddComponent<SpriteRenderer>();
                sr.color = new Color(0.08f, 0.08f, 0.15f);
                chunks[i] = chunk.transform;
            }
            var mapSO = new SerializedObject(mapCtrl);
            var chunksProp = mapSO.FindProperty("_chunks");
            chunksProp.arraySize = 9;
            for (int i = 0; i < 9; i++)
                chunksProp.GetArrayElementAtIndex(i).objectReferenceValue = chunks[i];
            mapSO.ApplyModifiedProperties();

            // --- Player ---
            var playerGO = new GameObject("Player");
            playerGO.tag = "Player";
            playerGO.layer = LayerMask.NameToLayer("Default");

            var playerSR = playerGO.AddComponent<SpriteRenderer>();
            playerSR.color = new Color(0.6f, 0.8f, 1f); // placeholder pastel blue

            var rb = playerGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = playerGO.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;

            var stats = playerGO.AddComponent<PlayerStats>();
            var controller = playerGO.AddComponent<PlayerController>();
            playerGO.AddComponent<PlayerXP>();

            // Link controller to stats
            var ctrlSO = new SerializedObject(controller);
            ctrlSO.FindProperty("_stats").objectReferenceValue = stats;
            ctrlSO.ApplyModifiedProperties();

            // Add initial RadialAttack
            playerGO.AddComponent<RadialAttack>();

            // Link camera to player
            var camFollowSO = new SerializedObject(cameraGO.GetComponent<CameraFollow>());
            camFollowSO.FindProperty("_target").objectReferenceValue = playerGO.transform;
            camFollowSO.ApplyModifiedProperties();

            // Link map to player
            mapSO.FindProperty("_playerTransform").objectReferenceValue = playerGO.transform;
            mapSO.ApplyModifiedProperties();

            // --- EventSystem ---
            if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            // --- Enemy Spawner ---
            var spawnerGO = new GameObject("EnemySpawner");
            var spawner = spawnerGO.AddComponent<EnemySpawner>();
            var spawnerSO = new SerializedObject(spawner);
            spawnerSO.FindProperty("_playerTransform").objectReferenceValue = playerGO.transform;
            spawnerSO.ApplyModifiedProperties();

            // --- UI: Canvas_Static (Timer, Pause) ---
            var canvasStatic = CreateCanvas("Canvas_Static", false);
            AddTimerUI(canvasStatic);

            // --- UI: Canvas_Dynamic (HP, XP, LevelUp, Result) ---
            var canvasDynamic = CreateCanvas("Canvas_Dynamic", true);
            AddHPBar(canvasDynamic, stats);
            AddXPBar(canvasDynamic);
            AddResultScreen(canvasDynamic);

            // Save scene
            UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            Debug.Log("[SceneSetup] MainGame scene setup complete! Assign SpawnTableData and Prefabs in Inspector.");
            EditorUtility.DisplayDialog("Scene Setup Complete",
                "MainGame scene is ready!\n\nTODO:\n• Assign enemy prefab to EnemySpawner\n• Assign SpawnTableData to EnemySpawner\n• Assign projectile prefab to RadialAttack\n• Run 'GlitchClaw/Generate All Game Data' if not done",
                "OK");
        }

        private static GameObject CreateManager<T>(string name) where T : MonoBehaviour
        {
            var go = new GameObject(name);
            go.AddComponent<T>();
            return go;
        }

        private static GameObject CreateCanvas(string name, bool renderOverlay)
        {
            var go = new GameObject(name);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = renderOverlay ? 10 : 5;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        private static void AddTimerUI(GameObject parent)
        {
            var go = new GameObject("TimerText");
            go.transform.SetParent(parent.transform, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = "10:00";
            text.fontSize = 36;
            text.alignment = TextAlignmentOptions.Center;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0, -20);
            rt.sizeDelta = new Vector2(200, 60);
            go.AddComponent<TimerUI>();
            var timerSO = new SerializedObject(go.GetComponent<TimerUI>());
            timerSO.FindProperty("_timerText").objectReferenceValue = text;
            timerSO.ApplyModifiedProperties();
        }

        private static void AddHPBar(GameObject parent, PlayerStats stats)
        {
            var go = new GameObject("HPBar");
            go.transform.SetParent(parent.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(20, 20);
            rt.sizeDelta = new Vector2(300, 30);
            var slider = go.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 1;
            var hpBar = go.AddComponent<HPBar>();
            var hpSO = new SerializedObject(hpBar);
            hpSO.FindProperty("_slider").objectReferenceValue = slider;
            hpSO.ApplyModifiedProperties();
        }

        private static void AddXPBar(GameObject parent)
        {
            var go = new GameObject("XPBar");
            go.transform.SetParent(parent.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(20, 60);
            rt.sizeDelta = new Vector2(300, 20);
            var slider = go.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 1;
            var xpBar = go.AddComponent<XPBar>();
            var xpSO = new SerializedObject(xpBar);
            xpSO.FindProperty("_slider").objectReferenceValue = slider;
            xpSO.ApplyModifiedProperties();
        }

        private static void AddResultScreen(GameObject parent)
        {
            var panelGO = new GameObject("ResultScreen");
            panelGO.transform.SetParent(parent.transform, false);
            var rt = panelGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var img = panelGO.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.85f);
            panelGO.SetActive(false);

            var rs = panelGO.AddComponent<ResultScreen>();
            var rsSO = new SerializedObject(rs);
            rsSO.FindProperty("_panelRoot").objectReferenceValue = panelGO;
            rsSO.ApplyModifiedProperties();
        }
    }
}
#endif
