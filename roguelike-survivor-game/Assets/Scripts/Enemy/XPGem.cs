using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// XP gem dropped by enemies. Auto-collected when player enters magnetic radius.
    /// </summary>
    public class XPGem : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _xpAmount = 5f;
        [SerializeField] private float _magnetRadius = 3f;
        [SerializeField] private float _collectSpeed = 8f;

        private Transform _playerTransform;
        private bool _isBeingAttracted;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb != null)
            {
                _rb.gravityScale = 0f;
                _rb.freezeRotation = true;
            }
        }

        public void OnSpawn()
        {
            _isBeingAttracted = false;
            // Player transform injected via SetPlayer(); fallback to tag search only once
            if (_playerTransform == null)
            {
                var p = GameObject.FindWithTag("Player");
                if (p != null) _playerTransform = p.transform;
            }
        }

        public void OnDespawn()
        {
            _isBeingAttracted = false;
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (_playerTransform == null) return;

            float distSq = (transform.position - _playerTransform.position).sqrMagnitude;

            if (distSq <= _magnetRadius * _magnetRadius)
                _isBeingAttracted = true;

            if (_isBeingAttracted)
            {
                Vector2 dir = (_playerTransform.position - transform.position).normalized;
                if (_rb != null)
                    _rb.MovePosition(_rb.position + dir * (_collectSpeed * Time.fixedDeltaTime));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            EventBus.RaiseXPCollected((int)_xpAmount);
            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(gameObject);
        }

        public void SetXPAmount(float amount) => _xpAmount = amount;
        public void SetPlayer(Transform player) => _playerTransform = player;
    }
}
