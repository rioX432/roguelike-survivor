using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Reads SpawnTableData and spawns enemies around the player over 10 minutes.
    /// Enemies spawn off-screen on a circle around the player.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private SpawnTableData _spawnTable;
        [SerializeField] private Transform _player;
        [SerializeField] private float _spawnRadius = 8f;      // distance from player (camera ortho=6)
        [SerializeField] private float _despawnRadius = 18f;   // despawn if too far

        // Prefabs keyed by EnemyData asset name — wire up in Inspector
        [SerializeField] private List<EnemyPrefabEntry> _enemyPrefabs = new();

        // Track active counts per enemy type
        private readonly Dictionary<string, int> _activeCounts = new();

        public void InjectRefs(Transform player, SpawnTableData spawnTable, List<EnemyPrefabEntry> prefabs)
        {
            _player = player;
            _spawnTable = spawnTable;
            _enemyPrefabs = prefabs;
        }
        // Track timers per wave
        private float[] _waveTimers;

        private float _gameTime;
        private bool _isRunning;

        private void Start()
        {
            if (_spawnTable == null || _spawnTable.waves == null) return;
            _waveTimers = new float[_spawnTable.waves.Count];
            _isRunning = true;
        }

        private void Update()
        {
            if (!_isRunning || _spawnTable == null) return;
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

            _gameTime = GameManager.Instance != null ? GameManager.Instance.ElapsedTime : 0f;

            for (int i = 0; i < _spawnTable.waves.Count; i++)
            {
                ProcessWave(i);
            }
        }

        private void ProcessWave(int index)
        {
            WaveEntry wave = _spawnTable.waves[index];
            if (wave.enemyData == null) return;
            if (_gameTime < wave.timeStart || _gameTime > wave.timeEnd) return;

            string key = wave.enemyData.enemyName;

            // Check active cap
            if (!_activeCounts.ContainsKey(key)) _activeCounts[key] = 0;
            if (_activeCounts[key] >= wave.maxActive) return;

            // Tick spawn timer
            _waveTimers[index] += Time.deltaTime;
            float spawnInterval = wave.spawnRate > 0f ? 1f / wave.spawnRate : float.MaxValue;

            if (_waveTimers[index] >= spawnInterval)
            {
                _waveTimers[index] = 0f;
                SpawnEnemy(wave.enemyData, key);
            }
        }

        private void SpawnEnemy(EnemyData data, string key)
        {
            if (_player == null || PoolManager.Instance == null) return;

            GameObject prefab = GetPrefabForEnemy(data.enemyName);
            if (prefab == null) return;

            Vector2 spawnPos = _player.position + (Vector3)(Random.insideUnitCircle.normalized * _spawnRadius);
            GameObject enemy = PoolManager.Instance.Spawn(prefab, spawnPos, Quaternion.identity);

            if (enemy.TryGetComponent<EnemyBase>(out var eb))
            {
                eb.Initialize(data, _player);
                if (!_activeCounts.ContainsKey(key)) _activeCounts[key] = 0;
                _activeCounts[key]++;
            }
        }

        /// <summary>Called by EnemyBase.Die() via EventBus to decrement active count.</summary>
        private void OnEnable()
        {
            EventBus.OnEnemyDeath += HandleEnemyDeath;
        }

        private void OnDisable()
        {
            EventBus.OnEnemyDeath -= HandleEnemyDeath;
        }

        private void HandleEnemyDeath(GameObject enemy)
        {
            if (enemy.TryGetComponent<EnemyBase>(out var eb))
            {
                string key = eb.Data != null ? eb.Data.enemyName : string.Empty;
                if (!string.IsNullOrEmpty(key) && _activeCounts.ContainsKey(key))
                {
                    _activeCounts[key] = Mathf.Max(0, _activeCounts[key] - 1);
                }
            }
        }

        private GameObject GetPrefabForEnemy(string enemyName)
        {
            foreach (var entry in _enemyPrefabs)
            {
                if (entry.enemyName == enemyName)
                    return entry.prefab;
            }
            return null;
        }

        [System.Serializable]
        public class EnemyPrefabEntry
        {
            public string enemyName;
            public GameObject prefab;
        }
    }
}
