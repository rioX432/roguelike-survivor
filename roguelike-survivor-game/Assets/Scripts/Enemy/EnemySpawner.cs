using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeSurvivor
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private SpawnTableData _spawnTable;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private float _spawnRadius = 12f;

        // Per-wave active count tracking (index matches _spawnTable.waves)
        private readonly List<int> _activeCountPerWave = new();
        private readonly List<float> _spawnTimers = new();

        private bool _isRunning;

        private void Start()
        {
            if (_spawnTable == null) return;

            for (int i = 0; i < _spawnTable.waves.Count; i++)
            {
                _activeCountPerWave.Add(0);
                _spawnTimers.Add(0f);
            }

            // Listen for enemy death to update active counts
            EventBus.OnEnemyDeath += OnEnemyDeath;
            _isRunning = true;
        }

        private void OnDestroy()
        {
            EventBus.OnEnemyDeath -= OnEnemyDeath;
        }

        private void Update()
        {
            if (!_isRunning || _spawnTable == null || GameManager.Instance == null) return;
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            float elapsed = GameManager.Instance.ElapsedTime;

            for (int i = 0; i < _spawnTable.waves.Count; i++)
            {
                WaveEntry wave = _spawnTable.waves[i];

                // Skip waves outside their time window
                if (elapsed < wave.timeStart || elapsed > wave.timeEnd) continue;

                // Skip waves with no enemy data assigned
                if (wave.enemyData == null) continue;

                // Skip if at cap
                if (_activeCountPerWave[i] >= wave.maxActive) continue;

                // Accumulate spawn timer
                _spawnTimers[i] += Time.deltaTime;
                float interval = wave.spawnRate > 0f ? 1f / wave.spawnRate : float.MaxValue;

                if (_spawnTimers[i] >= interval)
                {
                    _spawnTimers[i] -= interval;
                    SpawnEnemy(wave.enemyData, i);
                }
            }
        }

        private void SpawnEnemy(EnemyData data, int waveIndex)
        {
            if (_enemyPrefab == null || PoolManager.Instance == null || _playerTransform == null) return;

            Vector2 spawnPos = GetSpawnPositionAroundPlayer();

            GameObject obj = PoolManager.Instance.Spawn(
                _enemyPrefab,
                spawnPos,
                Quaternion.identity);

            if (obj.TryGetComponent<EnemyBase>(out var enemy))
                enemy.Initialize(data, _playerTransform);

            _activeCountPerWave[waveIndex]++;
        }

        private Vector2 GetSpawnPositionAroundPlayer()
        {
            // Random point on circle edge around player (off-screen)
            float angle = Random.Range(0f, Mathf.PI * 2f);
            return (Vector2)_playerTransform.position
                + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _spawnRadius;
        }

        private void OnEnemyDeath(GameObject _)
        {
            // Decrement all wave active counts by checking GameObject tags
            // Simple approach: just decrement the first wave that still has actives
            // More accurate tracking would require tagging enemies with wave index.
            for (int i = 0; i < _activeCountPerWave.Count; i++)
            {
                if (_activeCountPerWave[i] > 0)
                {
                    _activeCountPerWave[i]--;
                    break;
                }
            }
        }
    }
}
