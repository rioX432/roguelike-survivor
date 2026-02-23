using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Fires a spread of projectiles in a cone toward the nearest enemy (or player facing direction).
    /// </summary>
    public class ConeAttack : AttackModule
    {
        [SerializeField] private float _spreadAngle = 30f; // total spread in degrees

        protected override void Fire()
        {
            if (_data == null || _data.projectileCount <= 0) return;

            Vector2 aimDirection = GetAimDirection();
            float baseAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            int count = _data.projectileCount;
            float halfSpread = _spreadAngle * 0.5f;

            for (int i = 0; i < count; i++)
            {
                float t = count > 1 ? (float)i / (count - 1) : 0.5f;
                float angle = baseAngle - halfSpread + _spreadAngle * t;
                Vector2 dir = AngleToDirection(angle);

                float rot = angle;
                GameObject go = SpawnProjectile(transform.position, Quaternion.Euler(0, 0, rot));
                if (go != null && go.TryGetComponent<Projectile>(out var proj))
                {
                    proj.Setup(GetDamage(), _data.speed, _data.range, dir);
                }
            }
        }

        private Vector2 GetAimDirection()
        {
            // Aim at nearest enemy, fallback to player forward (up)
            GameObject nearest = FindNearestEnemy();
            if (nearest != null)
            {
                Vector2 toEnemy = nearest.transform.position - transform.position;
                if (toEnemy.sqrMagnitude > 0.001f) return toEnemy.normalized;
            }
            return Vector2.up;
        }

        private GameObject FindNearestEnemy()
        {
            // Use FindGameObjectsWithTag — cached is preferred but OK for non-hot-path (fired at intervals)
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

        private static Vector2 AngleToDirection(float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
    }
}
