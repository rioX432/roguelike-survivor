using System;

namespace RoguelikeSurvivor
{
    public static class EventBus
    {
        public static event Action<int> OnXPCollected;
        public static event Action<int> OnLevelUp;
        public static event Action OnPlayerDeath;
        public static event Action OnGameOver;
        public static event Action<EnemyBase> OnEnemyDeath;

        public static void RaiseXPCollected(int amount) => OnXPCollected?.Invoke(amount);
        public static void RaiseLevelUp(int level) => OnLevelUp?.Invoke(level);
        public static void RaisePlayerDeath() => OnPlayerDeath?.Invoke();
        public static void RaiseGameOver() => OnGameOver?.Invoke();
        public static void RaiseEnemyDeath(EnemyBase enemy) => OnEnemyDeath?.Invoke(enemy);

        public static void Clear()
        {
            OnXPCollected = null;
            OnLevelUp = null;
            OnPlayerDeath = null;
            OnGameOver = null;
            OnEnemyDeath = null;
        }
    }
}
