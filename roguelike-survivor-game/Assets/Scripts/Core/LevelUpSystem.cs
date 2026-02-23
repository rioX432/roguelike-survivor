using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Tracks XP, triggers level-up, pauses game and queues upgrade options.
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

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            EventBus.OnXPCollected += HandleXPCollected;
            EventBus.OnEnemyDeath += HandleEnemyDeath;
        }

        private void OnDisable()
        {
            EventBus.OnXPCollected -= HandleXPCollected;
            EventBus.OnEnemyDeath -= HandleEnemyDeath;
        }

        private void Start()
        {
            XPToNextLevel = CalculateXPThreshold(Level);
        }

        private void HandleXPCollected(int amount)
        {
            CurrentXP += amount;
            while (CurrentXP >= XPToNextLevel)
            {
                CurrentXP -= XPToNextLevel;
                LevelUp();
            }
        }

        private void HandleEnemyDeath(GameObject _)
        {
            _totalEnemiesKilled++;
        }

        private void LevelUp()
        {
            Level++;
            XPToNextLevel = CalculateXPThreshold(Level);
            EventBus.RaiseLevelUp(Level);

            // Pause and show upgrade UI
            if (GameManager.Instance != null)
                GameManager.Instance.SetState(GameState.LevelUp);

            if (LevelUpUI.Instance != null)
                LevelUpUI.Instance.Show(GetRandomUpgrades(3));
        }

        private float CalculateXPThreshold(int level)
        {
            return _baseXP * level * _xpScaling;
        }

        private List<WeaponData> GetRandomUpgrades(int count)
        {
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
    }
}
