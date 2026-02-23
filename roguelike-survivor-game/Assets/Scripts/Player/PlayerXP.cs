using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Tracks XP and level. Listens to EventBus.OnXPCollected.
    /// On level-up: pauses game and notifies EventBus.OnLevelUp.
    /// </summary>
    public class PlayerXP : MonoBehaviour
    {
        [SerializeField] private float _baseXPThreshold = 20f;
        [SerializeField] private float _levelScaling = 1.2f;

        public int Level { get; private set; } = 1;
        public float CurrentXP { get; private set; }
        public float XPThreshold { get; private set; }

        private void Start()
        {
            XPThreshold = _baseXPThreshold;
            EventBus.OnXPCollected += OnXPCollected;
        }

        private void OnDestroy()
        {
            EventBus.OnXPCollected -= OnXPCollected;
        }

        private void OnXPCollected(int amount)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState != GameState.Playing) return;

            CurrentXP += amount;

            while (CurrentXP >= XPThreshold)
            {
                CurrentXP -= XPThreshold;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            XPThreshold = _baseXPThreshold * Level * _levelScaling;

            // Pause and trigger level-up UI
            if (GameManager.Instance != null)
                GameManager.Instance.SetState(GameState.LevelUp);

            EventBus.RaiseLevelUp(Level);
        }

        /// <summary>Call when the player finishes picking an upgrade to resume.</summary>
        public void OnUpgradeChosen()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetState(GameState.Playing);
        }

        public float XPPercent => XPThreshold > 0f ? CurrentXP / XPThreshold : 0f;
    }
}
