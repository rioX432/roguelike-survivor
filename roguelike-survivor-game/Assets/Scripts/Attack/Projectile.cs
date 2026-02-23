using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Poolable projectile. Moves in a straight line until it hits an enemy or travels max range.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        /// <summary>Static prefab reference. Set by GameBootstrap or auto-detected on first spawn.</summary>
        public static GameObject PrefabInstance { get; private set; }

        public static void RegisterPrefab(GameObject prefab) => PrefabInstance = prefab;

        private Rigidbody2D _rb;
        private float _damage;
        private float _speed;
        private float _range;
        private float _traveledDistance;
        private Vector2 _direction;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
        }

        private void Start()
        {
            if (PrefabInstance == null)
                PrefabInstance = gameObject;
        }

        public void OnSpawn()
        {
            _traveledDistance = 0f;
        }

        public void OnDespawn()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        /// <summary>Initialize projectile after spawning.</summary>
        public void Setup(float damage, float speed, float range, Vector2 direction)
        {
            _damage = damage;
            _speed = speed;
            _range = range;
            _direction = direction.normalized;
            _rb.linearVelocity = _direction * _speed;
        }

        private void FixedUpdate()
        {
            _traveledDistance += _speed * Time.fixedDeltaTime;
            if (_traveledDistance >= _range)
            {
                Despawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return; // ignore player

            if (other.TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.TakeDamage(_damage);
                Despawn();
            }
        }

        private void Despawn()
        {
            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(gameObject);
        }
    }
}
