using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Fully self-contained runtime bootstrap.
    /// Creates ALL GameObjects, links ALL references, generates placeholder sprites.
    /// Just attach this to a single empty GameObject in the scene.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class GameBootstrap : MonoBehaviour
    {
        private GameObject _playerGO;
        private GameObject _enemyPrefabGO;
        private GameObject _projectilePrefabGO;
        private GameObject _xpGemPrefabGO;

        private void Awake()
        {
            // Skip if already initialized
            if (FindFirstObjectByType<GameManager>() != null) return;
            Bootstrap();
        }

        private void Bootstrap()
        {
            Debug.Log("[GameBootstrap] Starting runtime bootstrap...");

            CreateManagers();
            CreatePrefabs();
            CreatePlayer();
            CreateCamera();
            CreateMap();
            CreateHUD();
            CreateSpawner();
            StartCoroutine(WarmPoolDeferred());

            Debug.Log("[GameBootstrap] Done. Starting game.");

            // Kick off Playing state after one frame
            StartCoroutine(StartGameDeferred());
        }

        // ─── Managers ──────────────────────────────────────────────────────
        private void CreateManagers()
        {
            // Force-init TMP_Settings so TextMeshProUGUI works at runtime without asset file
            try
            {
                var existing = Resources.Load<TMP_Settings>("TMP Settings");
                if (existing == null)
                {
                    var s = ScriptableObject.CreateInstance<TMP_Settings>();
                    var field = typeof(TMP_Settings).GetField("s_Instance",
                        BindingFlags.Static | BindingFlags.NonPublic);
                    field?.SetValue(null, s);
                }
            }
            catch { /* TMP not available, text labels will be absent */ }

            // URP 2D: Global Light required for all sprites to be visible
            var lightGO = new GameObject("GlobalLight2D");
            var light = lightGO.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.intensity = 1f;
            light.color = Color.white;

            new GameObject("GameManager").AddComponent<GameManager>();
            new GameObject("PoolManager").AddComponent<PoolManager>();

            var amGO = new GameObject("AudioManager");
            amGO.AddComponent<AudioManager>();
            for (int i = 0; i < 6; i++) amGO.AddComponent<AudioSource>();

            var lu = new GameObject("LevelUpSystem").AddComponent<LevelUpSystem>();
            lu.SetWeapons(BuildWeaponList());
        }

        // ─── Prefabs (kept inactive, registered in pool) ───────────────────
        private void CreatePrefabs()
        {
            // ---- Enemy prefab ----
            _enemyPrefabGO = new GameObject("EnemyBase");
            var erb = _enemyPrefabGO.AddComponent<Rigidbody2D>();
            erb.gravityScale = 0f; erb.freezeRotation = true;
            var ec = _enemyPrefabGO.AddComponent<CircleCollider2D>();
            ec.radius = 0.45f; ec.isTrigger = true;
            AddUnlitSR(_enemyPrefabGO).sprite = LoadSprite("enemy_bit_drone") ?? CircleSprite(40, new Color(1f, 0.2f, 0.2f));
            _enemyPrefabGO.tag = "Enemy";
            _enemyPrefabGO.AddComponent<EnemyBase>(); // xpGemPrefab set after _xpGemPrefabGO created
            _enemyPrefabGO.SetActive(false);

            // ---- Projectile prefab ----
            _projectilePrefabGO = new GameObject("Projectile");
            var prb = _projectilePrefabGO.AddComponent<Rigidbody2D>();
            prb.gravityScale = 0f; prb.freezeRotation = true;
            var pc = _projectilePrefabGO.AddComponent<CircleCollider2D>();
            pc.radius = 0.15f; pc.isTrigger = true;
            AddUnlitSR(_projectilePrefabGO).sprite = LoadSprite("projectile") ?? CircleSprite(10, Color.cyan);
            _projectilePrefabGO.AddComponent<Projectile>();
            _projectilePrefabGO.SetActive(false);
            Projectile.RegisterPrefab(_projectilePrefabGO);

            // ---- XP Gem prefab ----
            _xpGemPrefabGO = new GameObject("XPGem");
            var grb = _xpGemPrefabGO.AddComponent<Rigidbody2D>();
            grb.gravityScale = 0f;
            var gc = _xpGemPrefabGO.AddComponent<CircleCollider2D>();
            gc.radius = 0.25f; gc.isTrigger = true;
            AddUnlitSR(_xpGemPrefabGO).sprite = LoadSprite("xp_gem") ?? DiamondSprite(16, Color.yellow);
            _xpGemPrefabGO.AddComponent<XPGem>();
            _xpGemPrefabGO.SetActive(false);

            // Inject xpGemPrefab into enemy base
            _enemyPrefabGO.GetComponent<EnemyBase>().SetXPGemPrefab(_xpGemPrefabGO);
        }

        // ─── Player ────────────────────────────────────────────────────────
        private void CreatePlayer()
        {
            _playerGO = new GameObject("Player");
            _playerGO.tag = "Player";
            _playerGO.transform.localScale = Vector3.one * 2.5f; // scale up for mobile visibility

            var rb = _playerGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; rb.freezeRotation = true;
            _playerGO.AddComponent<CircleCollider2D>().radius = 0.45f;
            AddUnlitSR(_playerGO).sprite = LoadSprite("player") ?? CircleSprite(56, new Color(0.5f, 0.8f, 1f));

            var stats = _playerGO.AddComponent<PlayerStats>();
            var ctrl = _playerGO.AddComponent<PlayerController>();
            ctrl.InjectStats(stats);

            _playerGO.AddComponent<PlayerXP>();

            // Start with radial attack
            var radial = _playerGO.AddComponent<RadialAttack>();
            radial.SetData(MakeWeapon("Radial Burst", AttackType.Radial, 20f, 1.5f, 9f, 9f, 8));
        }

        // ─── Camera ────────────────────────────────────────────────────────
        private void CreateCamera()
        {
            var camGO = GameObject.Find("Main Camera") ?? new GameObject("Main Camera");
            camGO.tag = "MainCamera";

            var cam = camGO.GetComponent<Camera>() ?? camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.04f, 0.04f, 0.12f);
            cam.transform.position = new Vector3(0, 0, -10);

            // URP 2D: ensure proper renderer assignment
            var urpData = camGO.GetComponent<UniversalAdditionalCameraData>();
            if (urpData == null) urpData = camGO.AddComponent<UniversalAdditionalCameraData>();
            urpData.renderType = CameraRenderType.Base;
            urpData.renderShadows = false;
            urpData.requiresColorOption = CameraOverrideOption.Off;
            urpData.requiresDepthOption = CameraOverrideOption.Off;

            var follow = camGO.GetComponent<CameraFollow>() ?? camGO.AddComponent<CameraFollow>();
            follow.InjectTarget(_playerGO.transform);

            Debug.Log("[GameBootstrap] Camera created ✓");
        }

        // ─── Map ───────────────────────────────────────────────────────────
        private void CreateMap()
        {
            var mapGO = new GameObject("Map");
            var ctrl = mapGO.AddComponent<InfiniteMapController>();
            ctrl.SetPlayer(_playerGO.transform);
            var bgSprite = LoadSprite("bg_tile");
            ctrl.SetTile(bgSprite != null ? bgSprite : MakeMapTile(256, 256));
        }

        // ─── HUD Canvas ────────────────────────────────────────────────────
        private void CreateHUD()
        {
            // EventSystem (required for touch/click)
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<EventSystem>();
                esGO.AddComponent<StandaloneInputModule>();
            }

            var canvasGO = new GameObject("Canvas_HUD");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            var root = canvasGO.GetComponent<RectTransform>();

            // ── HP Bar (top) ──
            var hpBarGO = CreateSliderGO("HPBar", root,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -30f), new Vector2(0f, -70f),
                new Color(0.8f, 0.1f, 0.1f));
            var hpBar = new GameObject("HPBarCtrl").AddComponent<HPBar>();
            hpBar.transform.SetParent(canvasGO.transform, false);
            hpBar.InjectSlider(hpBarGO.GetComponent<Slider>());

            // ── XP Bar (bottom) ──
            var xpBarGO = CreateSliderGO("XPBar", root,
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 20f), new Vector2(0f, 40f),
                new Color(0.9f, 0.8f, 0.1f));
            var xpBar = new GameObject("XPBarCtrl").AddComponent<XPBar>();
            xpBar.transform.SetParent(canvasGO.transform, false);
            xpBar.InjectSlider(xpBarGO.GetComponent<Slider>());

            // ── Level label (bottom-left) ──
            var levelTxt = MakeLabel("LevelLabel", root,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(90f, 70f), new Vector2(180f, 50f),
                "Lv.1", 40, TextAlignmentOptions.Left);
            xpBar.InjectLevelText(levelTxt);

            // ── Timer (top-right) ──
            var timerTxt = MakeLabel("TimerLabel", root,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-110f, -50f), new Vector2(220f, 60f),
                "10:00", 48, TextAlignmentOptions.Right);
            var timerUI = new GameObject("TimerUICtrl").AddComponent<TimerUI>();
            timerUI.transform.SetParent(canvasGO.transform, false);
            timerUI.InjectText(timerTxt);

            // ── Virtual Joystick ──
            VirtualJoystick joystick = CreateJoystick(root);
            _playerGO.GetComponent<PlayerController>().InjectJoystick(joystick);

            // ── Level-Up Panel ──
            CreateLevelUpPanel(root);

            // ── Result Screen ──
            CreateResultScreen(root);
        }

        // ─── Spawner ───────────────────────────────────────────────────────
        private void CreateSpawner()
        {
            var spawnerGO = new GameObject("EnemySpawner");
            var spawner = spawnerGO.AddComponent<EnemySpawner>();

            // Build runtime SpawnTableData
            var table = ScriptableObject.CreateInstance<SpawnTableData>();
            table.waves = new List<WaveEntry>
            {
                new WaveEntry { timeStart=0f,  timeEnd=60f,  enemyData=MakeEnemy("Bit Drone",   10f, 2.5f, 5f,  3f, 2.0f),  spawnRate=0.8f, maxActive=20 },
                new WaveEntry { timeStart=30f, timeEnd=180f, enemyData=MakeEnemy("Glitch Bug",  20f, 3.2f, 8f,  6f, 1.8f),  spawnRate=0.5f, maxActive=15 },
                new WaveEntry { timeStart=120f,timeEnd=360f, enemyData=MakeEnemy("Rust Walker", 50f, 1.8f, 12f, 10f,2.5f),  spawnRate=0.3f, maxActive=10 },
                new WaveEntry { timeStart=240f,timeEnd=480f, enemyData=MakeEnemy("Siege Core",  100f,1.2f, 18f, 15f,3.5f),  spawnRate=0.2f, maxActive=6  },
                new WaveEntry { timeStart=420f,timeEnd=600f, enemyData=MakeEnemy("Overlord AI", 300f,1.0f, 25f, 30f,5.0f),  spawnRate=0.1f, maxActive=3  },
            };

            // Register enemy prefab for each enemy type (all share base prefab, Initialize sets stats)
            var prefabs = new List<EnemySpawner.EnemyPrefabEntry>();
            foreach (var wave in table.waves)
                prefabs.Add(new EnemySpawner.EnemyPrefabEntry { enemyName = wave.enemyData.enemyName, prefab = _enemyPrefabGO });

            spawner.InjectRefs(_playerGO.transform, table, prefabs);
        }

        // ─── Pool Warm-up (deferred 1 frame) ──────────────────────────────
        private IEnumerator WarmPoolDeferred()
        {
            yield return null;
            if (PoolManager.Instance == null) yield break;
            PoolManager.Instance.PreWarm(_enemyPrefabGO, 40);
            PoolManager.Instance.PreWarm(_projectilePrefabGO, 80);
            PoolManager.Instance.PreWarm(_xpGemPrefabGO, 40);
            Debug.Log("[GameBootstrap] Pool warmed.");
        }

        private IEnumerator StartGameDeferred()
        {
            yield return null;
            yield return null; // 2 frames for all Start() methods to run
            if (GameManager.Instance != null)
                GameManager.Instance.SetState(GameState.Playing);
        }

        // ─── UI Helpers ────────────────────────────────────────────────────

        /// <summary>Creates a fullscreen background panel GO with a Slider fill.</summary>
        private GameObject CreateSliderGO(string name, RectTransform parent,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color fillColor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.05f, 0.05f, 0.05f, 0.7f);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin; rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin; rect.offsetMax = offsetMax;

            var fillArea = new GameObject("FillArea");
            fillArea.transform.SetParent(go.transform, false);
            var faRect = fillArea.AddComponent<RectTransform>();
            faRect.anchorMin = Vector2.zero; faRect.anchorMax = Vector2.one;
            faRect.offsetMin = new Vector2(2, 2); faRect.offsetMax = new Vector2(-2, -2);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = fillColor;
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = Vector2.zero; fillRect.offsetMax = Vector2.zero;

            var slider = go.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f; slider.maxValue = 1f; slider.value = 1f;
            slider.transition = Selectable.Transition.None;

            return go;
        }

        private TMP_Text MakeLabel(string name, RectTransform parent,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta,
            string text, float fontSize, TextAlignmentOptions align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = align;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin; rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = sizeDelta;
            return tmp;
        }

        private VirtualJoystick CreateJoystick(RectTransform parent)
        {
            var bgGO = new GameObject("JoystickBG");
            bgGO.transform.SetParent(parent, false);
            bgGO.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.12f);
            var bgRect = bgGO.GetComponent<RectTransform>();
            bgRect.anchorMin = bgRect.anchorMax = new Vector2(0.25f, 0.18f);
            bgRect.anchoredPosition = Vector2.zero;
            bgRect.sizeDelta = new Vector2(220, 220);

            var handleGO = new GameObject("JoystickHandle");
            handleGO.transform.SetParent(bgGO.transform, false);
            handleGO.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.45f);
            var handleRect = handleGO.GetComponent<RectTransform>();
            handleRect.anchorMin = handleRect.anchorMax = new Vector2(0.5f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(90, 90);

            // AddComponent triggers Awake — _background is null but we null-checked it
            var joystick = bgGO.AddComponent<VirtualJoystick>();
            joystick.InjectRects(bgRect, handleRect, 80f);
            return joystick;
        }

        private void CreateLevelUpPanel(RectTransform parent)
        {
            // Panel (visual, starts hidden)
            var panelGO = new GameObject("LevelUpPanel");
            panelGO.transform.SetParent(parent, false);
            panelGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.88f);
            var panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero; panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero; panelRect.offsetMax = Vector2.zero;
            panelGO.SetActive(false);

            MakeLabel("LUTitle", panelRect,
                new Vector2(0f, 0.72f), new Vector2(1f, 0.92f), Vector2.zero, Vector2.zero,
                "LEVEL UP!\n<size=50>Choose an upgrade</size>", 68, TextAlignmentOptions.Center);

            var cardBtns = new List<Button>();
            var cardTexts = new List<TMP_Text>();
            string[] cardNames = { "Radial Burst", "Cone Shot", "Homing Strike" };
            string[] cardDescs = { "360° auto attack", "Forward spread shot", "Seeks nearest enemy" };

            for (int i = 0; i < 3; i++)
            {
                float yAnchorMax = 0.68f - i * 0.23f;
                var cardGO = new GameObject($"Card_{i}");
                cardGO.transform.SetParent(panelGO.transform, false);
                var cardImg = cardGO.AddComponent<Image>();
                cardImg.color = new Color(0.1f, 0.1f, 0.35f, 0.95f);
                var cardRect = cardGO.GetComponent<RectTransform>();
                cardRect.anchorMin = new Vector2(0.07f, yAnchorMax - 0.20f);
                cardRect.anchorMax = new Vector2(0.93f, yAnchorMax);
                cardRect.offsetMin = Vector2.zero; cardRect.offsetMax = Vector2.zero;
                cardTexts.Add(MakeLabel($"CardText_{i}", cardRect,
                    Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                    $"<b>{cardNames[i]}</b>\n{cardDescs[i]}", 40, TextAlignmentOptions.Center));
                var btn = cardGO.AddComponent<Button>(); btn.targetGraphic = cardImg;
                cardBtns.Add(btn);
            }

            // Controller on ALWAYS-ACTIVE GO (not inside panel so Start() runs immediately)
            var luGO = new GameObject("LevelUpUICtrl");
            luGO.transform.SetParent(parent, false);
            var levelUpUI = luGO.AddComponent<LevelUpUI>();
            levelUpUI.InjectPanel(panelGO, cardBtns, cardTexts);
        }

        private void CreateResultScreen(RectTransform parent)
        {
            // Panel (visual, starts hidden)
            var panelGO = new GameObject("ResultPanel");
            panelGO.transform.SetParent(parent, false);
            panelGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.9f);
            var panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero; panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero; panelRect.offsetMax = Vector2.zero;
            panelGO.SetActive(false);

            MakeLabel("ResultTitle", panelRect,
                new Vector2(0.05f, 0.75f), new Vector2(0.95f, 0.92f), Vector2.zero, Vector2.zero,
                "GAME OVER", 90, TextAlignmentOptions.Center);
            var timeTxt = MakeLabel("TimeText", panelRect,
                new Vector2(0.1f, 0.60f), new Vector2(0.9f, 0.72f), Vector2.zero, Vector2.zero,
                "Survived: 00:00", 52, TextAlignmentOptions.Center);
            var killsTxt = MakeLabel("KillsText", panelRect,
                new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.60f), Vector2.zero, Vector2.zero,
                "Enemies: 0", 52, TextAlignmentOptions.Center);
            var levelTxt = MakeLabel("LevelText", panelRect,
                new Vector2(0.1f, 0.36f), new Vector2(0.9f, 0.48f), Vector2.zero, Vector2.zero,
                "Level: 1", 52, TextAlignmentOptions.Center);

            var btnGO = new GameObject("RetryBtn");
            btnGO.transform.SetParent(panelGO.transform, false);
            var btnImg = btnGO.AddComponent<Image>(); btnImg.color = new Color(0.1f, 0.6f, 0.15f);
            var btnRect = btnGO.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.2f, 0.16f); btnRect.anchorMax = new Vector2(0.8f, 0.30f);
            btnRect.offsetMin = Vector2.zero; btnRect.offsetMax = Vector2.zero;
            MakeLabel("RetryLabel", btnRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                "RETRY", 64, TextAlignmentOptions.Center);
            var retryBtn = btnGO.AddComponent<Button>(); retryBtn.targetGraphic = btnImg;

            // Controller on ALWAYS-ACTIVE GO
            var rsGO = new GameObject("ResultScreenCtrl");
            rsGO.transform.SetParent(parent, false);
            var rs = rsGO.AddComponent<ResultScreen>();
            rs.InjectUI(panelGO, timeTxt, killsTxt, levelTxt, retryBtn);
        }

        // ─── Data Factories ────────────────────────────────────────────────

        private static WeaponData MakeWeapon(string weaponName, AttackType type,
            float dmg, float interval, float speed, float range, int count)
        {
            var d = ScriptableObject.CreateInstance<WeaponData>();
            d.weaponName = weaponName;
            d.attackType = type;
            d.damage = dmg;
            d.interval = interval;
            d.speed = speed;
            d.range = range;
            d.projectileCount = count;
            return d;
        }

        private static EnemyData MakeEnemy(string enemyName,
            float hp, float speed, float contactDmg, float xp, float scale)
        {
            var d = ScriptableObject.CreateInstance<EnemyData>();
            d.enemyName = enemyName;
            d.maxHP = hp;
            d.moveSpeed = speed;
            d.contactDamage = contactDmg;
            d.xpDrop = xp;
            d.scale = scale;
            return d;
        }

        private static List<WeaponData> BuildWeaponList() => new List<WeaponData>
        {
            MakeWeapon("Radial Burst",   AttackType.Radial,  20f, 1.2f, 9f,  9f,  8),
            MakeWeapon("Cone Shot",      AttackType.Cone,    25f, 0.8f, 12f, 10f, 5),
            MakeWeapon("Homing Strike",  AttackType.Homing,  35f, 1.5f, 8f,  15f, 3),
            MakeWeapon("Burst Radial",   AttackType.Radial,  30f, 1.0f, 10f, 8f,  12),
            MakeWeapon("Wide Cone",      AttackType.Cone,    20f, 0.6f, 11f, 12f, 7),
            MakeWeapon("Multi-Homing",   AttackType.Homing,  25f, 1.0f, 9f,  18f, 5),
        };

        // ─── Unlit Material Helper ─────────────────────────────────────────
        // URP 2D: Sprite-Lit-Default requires Light2D; use Unlit so sprites
        // are always visible regardless of light setup.

        private static Material _unlitMat;

        private static SpriteRenderer AddUnlitSR(GameObject go)
        {
            var sr = go.AddComponent<SpriteRenderer>();
            if (_unlitMat == null)
            {
                var sh = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                      ?? Shader.Find("Sprites/Default");
                if (sh != null) _unlitMat = new Material(sh);
            }
            if (_unlitMat != null) sr.material = _unlitMat;
            return sr;
        }

        // ─── Resource Sprite Loader ────────────────────────────────────────

        private static Sprite LoadSprite(string name)
        {
            var sp = Resources.Load<Sprite>($"Sprites/{name}");
            if (sp != null) Debug.Log($"[GameBootstrap] Loaded sprite: {name}");
            return sp;
        }

        // ─── Sprite Factories ──────────────────────────────────────────────

        private static Sprite CircleSprite(int size, Color color)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float r = size * 0.5f;
            float r2 = r * r;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = x - r + 0.5f, dy = y - r + 0.5f;
                    float dist2 = dx * dx + dy * dy;
                    float a = Mathf.Clamp01((r2 - dist2) / (r * 2f));
                    tex.SetPixel(x, y, dist2 < r2 ? new Color(color.r, color.g, color.b, a) : Color.clear);
                }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f, size);
        }

        private static Sprite DiamondSprite(int size, Color color)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float h = size * 0.5f;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    tex.SetPixel(x, y, Mathf.Abs(x - h) + Mathf.Abs(y - h) < h ? color : Color.clear);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f, size);
        }

        private static Sprite MakeMapTile(int w, int h)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var bgColor = new Color(0.05f, 0.04f, 0.14f);
            var gridColor = new Color(0.18f, 0.04f, 0.38f, 0.7f);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    tex.SetPixel(x, y, (x % 64 < 2) || (y % 64 < 2) ? gridColor : bgColor);
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Repeat;
            // 256px covers 12 world units (matches InfiniteMapController default _chunkSize=12)
            return Sprite.Create(tex, new Rect(0, 0, w, h), Vector2.one * 0.5f, w / 12f);
        }
    }
}
