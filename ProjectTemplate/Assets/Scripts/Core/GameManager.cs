using UnityEngine;

namespace RoguelikeSurvivor
{
    public enum GameState
    {
        Playing,
        Paused,
        LevelUp,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private float gameDuration = 600f; // 10 minutes

        public GameState CurrentState { get; private set; }
        public float ElapsedTime { get; private set; }
        public float RemainingTime => Mathf.Max(0f, gameDuration - ElapsedTime);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            CurrentState = GameState.Playing;
            ElapsedTime = 0f;
        }

        private void Update()
        {
            if (CurrentState != GameState.Playing) return;

            ElapsedTime += Time.deltaTime;

            if (ElapsedTime >= gameDuration)
            {
                SetState(GameState.GameOver);
                EventBus.RaiseGameOver();
            }
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            Time.timeScale = newState == GameState.Playing ? 1f : 0f;
        }
    }
}
