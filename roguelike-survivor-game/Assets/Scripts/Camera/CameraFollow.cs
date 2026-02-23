using UnityEngine;

namespace RoguelikeSurvivor
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private float _orthographicSize = 5f;

        public void InjectTarget(Transform target) => _target = target;

        private const float CameraZ = -10f;

        private void Start()
        {
            if (Camera.main != null)
            {
                Camera.main.orthographicSize = _orthographicSize;
            }

            // Snap to target immediately on start (no initial lerp lag)
            if (_target != null)
            {
                Vector3 targetPos = _target.position;
                transform.position = new Vector3(targetPos.x, targetPos.y, CameraZ);
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 targetPosition = new Vector3(_target.position.x, _target.position.y, CameraZ);
            float alpha = Mathf.Clamp01(_smoothSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPosition, alpha);
        }
    }
}
