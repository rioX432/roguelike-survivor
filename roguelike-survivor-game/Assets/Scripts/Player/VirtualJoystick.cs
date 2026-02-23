using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeSurvivor
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private float _range = 75f;

        public Vector2 Direction { get; private set; }

        private Vector2 _startPosition;
        private Canvas _canvas;

        public void InjectRects(RectTransform background, RectTransform handle, float range)
        {
            _background = background;
            _handle = handle;
            _range = range;
            _startPosition = background.anchoredPosition;
        }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            if (_background != null) _startPosition = _background.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Fixed joystick: don't move the background to avoid bottom-edge issues
            // Reset handle to center on new touch
            _handle.anchoredPosition = Vector2.zero;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint)) return;

            Vector2 offset = localPoint;
            float magnitude = offset.magnitude;

            if (magnitude > _range)
            {
                offset = offset.normalized * _range;
            }

            _handle.anchoredPosition = offset;
            Direction = offset / _range;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Direction = Vector2.zero;
            _handle.anchoredPosition = Vector2.zero;
            _background.anchoredPosition = _startPosition;
        }
    }
}
