using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Homing variant of Projectile that steers toward the nearest enemy.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class HomingProjectile : MonoBehaviour, IPoolable
    {
        private Rigidbody2D _rb;
        private float _damage;
        private float _speed;
        private float _range;
        private float _traveledDistance;
        private float _homingStrength;
        private Vector2 _direction;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
        }

        public void OnSpawn() => _traveledDistance = 0f;
        public void OnDespawn() => _rb.linearVelocity = Vector2.zero;

        public void Setup(float damage, float speed, float range, Vector2 direction, float homingStrength)
        {
            _damage = damage;
            _speed = speed;
            _range = range;
            _direction = direction.normalized;
            _homingStrength = homingStrength;
            _rb.linearVelocity = _direction * _speed;
        }

        private void FixedUpdate()
        {
            _traveledDistance += _speed * Time.fixedDeltaTime;
            if (_traveledDistance >= _range) { Despawn(); return; }

            // Steer toward nearest enemy
            GameObject nearest = FindNearestEnemy();
            if (nearest != null)
            {
                Vector2 toTarget = ((Vector2)nearest.transform.position - _rb.position).normalized;
                _direction = Vector2.Lerp(_direction, toTarget, _homingStrength * Time.fixedDeltaTime).normalized;
            }

            _rb.linearVelocity = _direction * _speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return;
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

        private static GameObject FindNearestEnemy()
        {
            // FindGameObjectsWithTag is OK here (called at fire interval, not every frame)
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return null;
            GameObject nearest = null;
            float minSq = float.MaxValue;
            foreach (var e in enemies)
            {
                float dSq = e.transform.position.sqrMagnitude;
                if (dSq < minSq) { minSq = dSq; nearest = e; }
            }
            return nearest;
        }
    }
}
