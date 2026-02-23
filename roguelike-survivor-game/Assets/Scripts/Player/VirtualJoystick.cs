using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Full-screen swipe input: touch anywhere, drag to move.
    /// The direction is calculated from the initial touch point.
    /// No visible joystick UI.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        /// <summary>Normalized movement direction (magnitude 0–1).</summary>
        public Vector2 Direction { get; private set; }

        [SerializeField] private float _deadZone = 8f;      // pixels before movement starts
        [SerializeField] private float _fullRange = 120f;   // pixels for full-speed input

        private Vector2 _touchOrigin;
        private bool _isTouching;

        // InjectRects kept for API compatibility — no-op in swipe mode
        public void InjectRects(RectTransform background, RectTransform handle, float range)
        {
            _fullRange = range;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _touchOrigin = eventData.position;
            _isTouching = true;
            Direction = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isTouching) return;

            Vector2 delta = eventData.position - _touchOrigin;
            float magnitude = delta.magnitude;

            if (magnitude < _deadZone)
            {
                Direction = Vector2.zero;
                return;
            }

            // Normalize direction; scale magnitude to 0–1 over fullRange
            float strength = Mathf.Clamp01((magnitude - _deadZone) / (_fullRange - _deadZone));
            Direction = delta.normalized * strength;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Direction = Vector2.zero;
            _isTouching = false;
        }
    }
}
