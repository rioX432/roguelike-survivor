using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Fires a spread of projectiles in a forward-facing cone.
    /// "Forward" is determined by the player's last move direction.
    /// Falls back to world-up if the player is stationary.
    /// </summary>
    public class ConeAttack : AttackModule
    {
        [SerializeField] private float _spreadAngle = 45f;

        private PlayerController _playerController;

        // Track last non-zero move dir to know "forward"
        private Vector2 _lastMoveDir = Vector2.up;

        private void Awake()
        {
            TryGetComponent(out _playerController);
        }

        protected override void Fire()
        {
            if (_weaponData == null) return;

            int count = Mathf.Max(1, _weaponData.projectileCount);

            // Spread evenly across the cone angle
            float halfSpread = _spreadAngle * 0.5f;
            float step = count > 1 ? _spreadAngle / (count - 1) : 0f;

            float baseAngle = Mathf.Atan2(_lastMoveDir.y, _lastMoveDir.x) * Mathf.Rad2Deg;

            for (int i = 0; i < count; i++)
            {
                float offsetDeg = count > 1 ? -halfSpread + step * i : 0f;
                float rad = (baseAngle + offsetDeg) * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                SpawnProjectile(dir);
            }
        }

        /// <summary>Call from PlayerController each frame to keep facing direction fresh.</summary>
        public void SetFacingDirection(Vector2 dir)
        {
            if (dir.sqrMagnitude > 0.01f)
                _lastMoveDir = dir.normalized;
        }
    }
}
