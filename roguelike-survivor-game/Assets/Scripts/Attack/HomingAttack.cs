using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Fires a projectile that homes toward the nearest enemy.
    /// The homing direction is set at launch time — projectiles fly straight,
    /// but always start aimed at the current nearest target.
    /// </summary>
    public class HomingAttack : AttackModule
    {
        [SerializeField] private float _detectionRadius = 15f;

        private readonly Collider2D[] _overlapBuffer = new Collider2D[64];

        protected override void Fire()
        {
            if (_weaponData == null) return;

            Vector2 targetDir = FindNearestEnemyDirection();
            if (targetDir == Vector2.zero)
                targetDir = Vector2.up; // fallback: shoot upward

            SpawnProjectile(targetDir);
        }

        private Vector2 FindNearestEnemyDirection()
        {
            // Use OverlapCircleNonAlloc to avoid GC in hot path
            int count = Physics2D.OverlapCircleNonAlloc(
                transform.position, _detectionRadius, _overlapBuffer);

            float bestSqrDist = float.MaxValue;
            Vector2 bestDir = Vector2.zero;

            for (int i = 0; i < count; i++)
            {
                Collider2D col = _overlapBuffer[i];
                if (col == null) continue;
                if (!col.CompareTag("Enemy")) continue;

                Vector2 toTarget = (Vector2)(col.transform.position - transform.position);
                float sqrDist = toTarget.sqrMagnitude;
                if (sqrDist < bestSqrDist)
                {
                    bestSqrDist = sqrDist;
                    bestDir = toTarget.normalized;
                }
            }

            return bestDir;
        }
    }
}
