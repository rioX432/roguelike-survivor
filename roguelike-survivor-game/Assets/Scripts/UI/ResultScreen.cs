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

        private float _survivalTime;

        public void InjectUI(GameObject panel, TMP_Text time, TMP_Text kills, TMP_Text level, Button retry)
        {
            _panelRoot = panel;
            _survivalTimeText = time;
            _enemiesKilledText = kills;
            _levelReachedText = level;
            _retryButton = retry;
        }

        private void Start()
        {
            _panelRoot.SetActive(false);
            _retryButton?.onClick.AddListener(OnRetry);
        }

        private void OnEnable()
        {
            EventBus.OnGameOver += ShowResult;
            EventBus.OnPlayerDeath += ShowResult;
        }

        private void OnDisable()
        {
            EventBus.OnGameOver -= ShowResult;
            EventBus.OnPlayerDeath -= ShowResult;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
                _survivalTime = GameManager.Instance.ElapsedTime;
        }

        private void ShowResult()
        {
            _panelRoot.SetActive(true);

            int minutes = Mathf.FloorToInt(_survivalTime / 60f);
            int seconds = Mathf.FloorToInt(_survivalTime % 60f);
            if (_survivalTimeText != null)
                _survivalTimeText.text = $"Survived: {minutes:00}:{seconds:00}";

            int kills = LevelUpSystem.Instance != null ? LevelUpSystem.Instance.GetEnemiesKilled() : 0;
            if (_enemiesKilledText != null)
                _enemiesKilledText.text = $"Enemies: {kills}";

            int level = LevelUpSystem.Instance != null ? LevelUpSystem.Instance.Level : 1;
            if (_levelReachedText != null)
                _levelReachedText.text = $"Level: {level}";
        }

        private void OnRetry()
        {
            Time.timeScale = 1f;
            EventBus.Clear();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
