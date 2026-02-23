using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Fires a single homing projectile that seeks the nearest enemy.
    /// </summary>
    public class HomingAttack : AttackModule
    {
        [SerializeField] private float _homingStrength = 5f; // turn speed in deg/sec

        protected override void Fire()
        {
            if (_data == null) return;

            GameObject target = FindNearestEnemy();
            Vector2 dir = target != null
                ? ((Vector2)(target.transform.position - transform.position)).normalized
                : Vector2.up;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            GameObject go = SpawnProjectile(transform.position, Quaternion.Euler(0, 0, angle));
            if (go != null && go.TryGetComponent<HomingProjectile>(out var proj))
            {
                proj.Setup(GetDamage(), _data.speed, _data.range, dir, _homingStrength);
            }
            else if (go != null && go.TryGetComponent<Projectile>(out var basicProj))
            {
                basicProj.Setup(GetDamage(), _data.speed, _data.range, dir);
            }
        }

        private GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return null;

            GameObject nearest = null;
            float minDistSq = float.MaxValue;
            foreach (var e in enemies)
            {
                float dSq = (e.transform.position - transform.position).sqrMagnitude;
                if (dSq < minDistSq) { minDistSq = dSq; nearest = e; }
            }
            return nearest;
        }
    }
}
