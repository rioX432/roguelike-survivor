using UnityEngine;

namespace RoguelikeSurvivor
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private float _maxHP = 100f;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _attackPower = 10f;
        [SerializeField] private float _attackSpeed = 1f;
        [SerializeField] private float _invincibilityDuration = 0.5f;

        public float MaxHP => _maxHP;
        public float CurrentHP { get; private set; }
        public float MoveSpeed => _moveSpeed;
        public float AttackPower => _attackPower;
        public float AttackSpeed => _attackSpeed;
        public bool IsInvincible => _invincibilityTimer > 0f;

        private float _invincibilityTimer;

        private void Awake()
        {
            CurrentHP = _maxHP;
        }

        private void Update()
        {
            if (_invincibilityTimer > 0f)
            {
                _invincibilityTimer -= Time.deltaTime;
            }
        }

        public void TakeDamage(float amount)
        {
            if (IsInvincible) return;

            AudioManager.Instance?.PlayPlayerHit();
            CurrentHP -= amount;
            _invincibilityTimer = _invincibilityDuration;

            if (CurrentHP <= 0f)
            {
                CurrentHP = 0f;
                EventBus.RaisePlayerDeath();
            }
        }

        public void Heal(float amount)
        {
            CurrentHP = Mathf.Min(CurrentHP + amount, _maxHP);
        }
    }
}
