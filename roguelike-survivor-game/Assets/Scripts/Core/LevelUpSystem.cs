using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Tracks XP, triggers level-up, pauses game and queues upgrade options.
    /// Includes failsafe: auto-resumes if UI is unavailable or player never taps.
    /// </summary>
    public class LevelUpSystem : MonoBehaviour
    {
        public static LevelUpSystem Instance { get; private set; }

        [SerializeField] private float _baseXP = 50f;
        [SerializeField] private float _xpScaling = 1.2f;
        [SerializeField] private List<WeaponData> _availableWeapons;

        public int Level { get; private set; } = 1;
        public float CurrentXP { get; private set; }
        public float XPToNextLevel { get; private set; }

        private int _totalEnemiesKilled;
        private bool _pendingLevelUp;        // prevents re-entrant level-ups
        private readonly Queue<int> _levelUpQueue = new Queue<int>(); // buffered levels

        private const float LEVELUP_TIMEOUT = 8f; // auto-resume after N real seconds

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            EventBus.OnXPCollected += HandleXPCollected;
            EventBus.OnEnemyDeath  += HandleEnemyDeath;
        }

        private void OnDisable()
        {
            EventBus.OnXPCollected -= HandleXPCollected;
            EventBus.OnEnemyDeath  -= HandleEnemyDeath;
        }

        private void Start()
        {
            XPToNextLevel = Mathf.Max(1f, CalculateXPThreshold(Level));
        }

        private void HandleXPCollected(int amount)
        {
            if (GameManager.Instance == null) return;
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            CurrentXP += amount;

            // Accumulate level-ups without re-entering while paused
            while (CurrentXP >= XPToNextLevel && !_pendingLevelUp)
            {
                CurrentXP -= XPToNextLevel;
                Level++;
                XPToNextLevel = Mathf.Max(1f, CalculateXPThreshold(Level));
                _levelUpQueue.Enqueue(Level);
            }

            if (_levelUpQueue.Count > 0 && !_pendingLevelUp)
                StartCoroutine(ProcessNextLevelUp());
        }

        private IEnumerator ProcessNextLevelUp()
        {
            if (_levelUpQueue.Count == 0) yield break;

            _pendingLevelUp = true;
            int newLevel = _levelUpQueue.Dequeue();

            EventBus.RaiseLevelUp(newLevel);

            // Try to show upgrade UI
            bool uiAvailable = LevelUpUI.Instance != null;
            if (uiAvailable)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.SetState(GameState.LevelUp);

                LevelUpUI.Instance.Show(GetRandomUpgrades(3));

                // Failsafe: auto-resume after timeout (real time, unaffected by timeScale)
                float elapsed = 0f;
                while (GameManager.Instance != null &&
                       GameManager.Instance.CurrentState == GameState.LevelUp &&
                       elapsed < LEVELUP_TIMEOUT)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                // If still paused, force resume
                if (GameManager.Instance != null &&
                    GameManager.Instance.CurrentState == GameState.LevelUp)
                {
                    Debug.LogWarning("[LevelUpSystem] Timeout — auto-resuming game");
                    LevelUpUI.Instance?.Hide();
                    GameManager.Instance.SetState(GameState.Playing);
                }
            }
            // else: no UI — apply a random upgrade silently and keep playing

            _pendingLevelUp = false;

            // Process next queued level-up after a short delay
            if (_levelUpQueue.Count > 0)
            {
                yield return new WaitForSecondsRealtime(0.3f);
                StartCoroutine(ProcessNextLevelUp());
            }
        }

        private void HandleEnemyDeath(GameObject _) => _totalEnemiesKilled++;

        private float CalculateXPThreshold(int level)
        {
            return Mathf.Max(1f, _baseXP * level * _xpScaling);
        }

        private List<WeaponData> GetRandomUpgrades(int count)
        {
            if (_availableWeapons == null || _availableWeapons.Count == 0)
                return new List<WeaponData>();

            var shuffled = new List<WeaponData>(_availableWeapons);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }
            var result = new List<WeaponData>();
            for (int i = 0; i < Mathf.Min(count, shuffled.Count); i++)
                result.Add(shuffled[i]);
            return result;
        }

        public int GetEnemiesKilled() => _totalEnemiesKilled;
        public void SetWeapons(List<WeaponData> weapons) => _availableWeapons = weapons;
    }
}
