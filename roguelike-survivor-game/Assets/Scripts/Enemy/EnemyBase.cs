using UnityEngine;

namespace RoguelikeSurvivor
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyBase : MonoBehaviour, IPoolable
    {
        [SerializeField] protected EnemyData _data;

        public EnemyData Data => _data;
        public float CurrentHP { get; private set; }
        public bool IsAlive => CurrentHP > 0f;

        protected Transform _playerTransform;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;

        // Cache the move direction to avoid per-frame allocation
        private Vector2 _moveDirection;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            // Rigidbody setup for 2D top-down
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public virtual void OnSpawn()
        {
            if (_data == null) return;

            CurrentHP = _data.maxHP;
            transform.localScale = Vector3.one * _data.scale;

            if (_spriteRenderer != null && _data.sprite != null)
                _spriteRenderer.sprite = _data.sprite;

            // Find player every spawn (player is persistent, reference is stable)
            if (_playerTransform == null)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    _playerTransform = playerObj.transform;
            }
        }

        public virtual void OnDespawn()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        protected virtual void FixedUpdate()
        {
            if (!IsAlive || _playerTransform == null || _data == null) return;
            ChasePlayer();
        }

        protected virtual void ChasePlayer()
        {
            _moveDirection = (_playerTransform.position - transform.position);
            if (_moveDirection.sqrMagnitude > 0.001f)
                _moveDirection.Normalize();

            _rb.MovePosition(_rb.position + _moveDirection * (_data.moveSpeed * Time.fixedDeltaTime));
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsAlive) return;
            if (!other.CompareTag("Player")) return;

            if (other.TryGetComponent<PlayerStats>(out var playerStats))
            {
                playerStats.TakeDamage(_data.contactDamage);
            }
        }

        public virtual void TakeDamage(float amount)
        {
            if (!IsAlive) return;

            CurrentHP -= amount;
            if (CurrentHP <= 0f)
            {
                CurrentHP = 0f;
                Die();
            }
        }

        protected virtual void Die()
        {
            EventBus.RaiseEnemyDeath(gameObject);

            // Drop XP gem via pool
            if (PoolManager.Instance != null && XPGem.Prefab != null)
            {
                PoolManager.Instance.Spawn(XPGem.Prefab, transform.position, Quaternion.identity);
            }

            // Return to pool
            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(gameObject);
        }
    }
}
