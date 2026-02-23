using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Abstract base class for all attack modules attached to the player.
    /// Modules fire automatically at configured intervals.
    /// </summary>
    public abstract class AttackModule : MonoBehaviour
    {
        [SerializeField] protected WeaponData _data;

        protected float _timer;
        protected PlayerStats _playerStats;

        protected virtual void Awake()
        {
            _playerStats = GetComponentInParent<PlayerStats>();
        }

        protected virtual void Update()
        {
            if (_data == null) return;
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

            _timer += Time.deltaTime;

            float adjustedInterval = _data.interval;
            if (_playerStats != null)
                adjustedInterval /= _playerStats.AttackSpeed;

            if (_timer >= adjustedInterval)
            {
                _timer = 0f;
                Fire();
            }
        }

        protected abstract void Fire();

        public void SetData(WeaponData data) => _data = data;

        /// <summary>Effective damage considering player stat bonuses.</summary>
        protected float GetDamage()
        {
            float bonus = _playerStats != null ? _playerStats.AttackPower : 0f;
            return _data.damage + bonus;
        }

        protected GameObject SpawnProjectile(Vector3 position, Quaternion rotation)
        {
            if (PoolManager.Instance == null || Projectile.PrefabInstance == null) return null;
            return PoolManager.Instance.Spawn(Projectile.PrefabInstance, position, rotation);
        }
    }
}
