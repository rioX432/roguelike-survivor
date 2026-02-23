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

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _startPosition = _background.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
            {
                _background.anchoredPosition = localPoint;
            }
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
