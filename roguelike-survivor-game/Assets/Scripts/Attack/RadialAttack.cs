using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Fires projectiles in all directions (360°) at a fixed interval.
    /// </summary>
    public class RadialAttack : AttackModule
    {
        protected override void Fire()
        {
            if (_data == null || _data.projectileCount <= 0) return;

            float angleStep = 360f / _data.projectileCount;
            float startAngle = Time.time * 30f; // slow rotation for visual variety

            for (int i = 0; i < _data.projectileCount; i++)
            {
                float angle = startAngle + angleStep * i;
                Vector2 dir = AngleToDirection(angle);
                SpawnAndSetupProjectile(dir);
            }
        }

        private void SpawnAndSetupProjectile(Vector2 direction)
        {
            if (Projectile.PrefabInstance == null) return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject go = SpawnProjectile(transform.position, Quaternion.Euler(0, 0, angle));
            if (go != null && go.TryGetComponent<Projectile>(out var proj))
            {
                proj.Setup(GetDamage(), _data.speed, _data.range, direction);
            }
        }

        private static Vector2 AngleToDirection(float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
    }
}
