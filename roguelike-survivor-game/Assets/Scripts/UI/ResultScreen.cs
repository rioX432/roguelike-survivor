using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace RoguelikeSurvivor
{
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private TMP_Text _survivalTimeText;
        [SerializeField] private TMP_Text _enemiesKilledText;
        [SerializeField] private TMP_Text _levelReachedText;
        [SerializeField] private Button _retryButton;

        private int _enemyKillCount;
        private float _survivalTime;

        private void Start()
        {
            _panelRoot.SetActive(false);

            EventBus.OnGameOver += OnGameOver;
            EventBus.OnPlayerDeath += OnGameOver;
            EventBus.OnEnemyDeath += OnEnemyDeath;

            _retryButton?.onClick.AddListener(OnRetry);
        }

        private void OnDestroy()
        {
            EventBus.OnGameOver -= OnGameOver;
            EventBus.OnPlayerDeath -= OnGameOver;
            EventBus.OnEnemyDeath -= OnEnemyDeath;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
                _survivalTime = GameManager.Instance.ElapsedTime;
        }

        private void OnEnemyDeath(GameObject _)
        {
            _enemyKillCount++;
        }

        private void OnGameOver()
        {
            ShowResult();
        }

        private void ShowResult()
        {
            _panelRoot.SetActive(true);

            int minutes = Mathf.FloorToInt(_survivalTime / 60f);
            int seconds = Mathf.FloorToInt(_survivalTime % 60f);
            if (_survivalTimeText != null)
                _survivalTimeText.text = $"Survived: {minutes:00}:{seconds:00}";

            if (_enemiesKilledText != null)
                _enemiesKilledText.text = $"Enemies: {_enemyKillCount}";

            // Get level from PlayerXP
            var playerXP = FindFirstObjectByType<PlayerXP>();
            if (_levelReachedText != null)
                _levelReachedText.text = $"Level: {(playerXP != null ? playerXP.Level : 1)}";
        }

        private void OnRetry()
        {
            Time.timeScale = 1f;
            EventBus.Clear();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
