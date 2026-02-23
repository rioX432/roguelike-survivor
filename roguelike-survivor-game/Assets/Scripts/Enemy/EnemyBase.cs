using UnityEngine;

namespace RoguelikeSurvivor
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyBase : MonoBehaviour, IPoolable
    {
        [SerializeField] private GameObject _xpGemPrefab;

        private EnemyData _data;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private Transform _playerTransform;

        private float _currentHP;
        private bool _isDead;

        // Cached direction — no allocation in FixedUpdate
        private Vector2 _moveDir;

        public EnemyData Data => _data;

        public void SetXPGemPrefab(GameObject prefab) => _xpGemPrefab = prefab;

        public void Initialize(EnemyData data, Transform playerTransform)
        {
            _data = data;
            _playerTransform = playerTransform;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            TryGetComponent(out _spriteRenderer);
        }

        public void OnSpawn()
        {
            if (_data == null) return;
            _currentHP = _data.maxHP;
            _isDead = false;
            transform.localScale = Vector3.one * _data.scale;
            if (_spriteRenderer != null && _data.sprite != null)
                _spriteRenderer.sprite = _data.sprite;
        }

        public void OnDespawn()
        {
            _isDead = false;
            _moveDir = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (_isDead || _data == null || _playerTransform == null) return;

            // Chase AI: move toward player — no allocation
            _moveDir = (Vector2)(_playerTransform.position - transform.position);
            if (_moveDir.sqrMagnitude > 0.01f)
                _moveDir.Normalize();

            _rb.MovePosition(_rb.position + _moveDir * (_data.moveSpeed * Time.fixedDeltaTime));
        }

        public void TakeDamage(float amount)
        {
            if (_isDead) return;
            _currentHP -= amount;
            if (_currentHP <= 0f)
                Die();
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;

            // Drop XP gem
            DropXPGem();

            EventBus.RaiseEnemyDeath(gameObject);

            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(gameObject);
            else
                gameObject.SetActive(false);
        }

        private void DropXPGem()
        {
            if (_xpGemPrefab == null || _data == null || PoolManager.Instance == null) return;

            GameObject gem = PoolManager.Instance.Spawn(_xpGemPrefab, transform.position, Quaternion.identity);
            if (gem.TryGetComponent<XPGem>(out var xpGem))
                xpGem.SetXPAmount(_data.xpDrop);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isDead) return;
            if (!other.CompareTag("Player")) return;

            if (other.TryGetComponent<PlayerStats>(out var stats))
                stats.TakeDamage(_data.contactDamage);
        }
    }
}
