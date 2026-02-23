using UnityEngine;
using UnityEngine.InputSystem;

namespace RoguelikeSurvivor
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStats _stats;
        [SerializeField] private VirtualJoystick _virtualJoystick;

        public void InjectStats(PlayerStats stats) => _stats = stats;
        public void InjectJoystick(VirtualJoystick joystick) => _virtualJoystick = joystick;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_stats == null) TryGetComponent(out _stats);
        }

        private void Update()
        {
            // Virtual joystick (mobile)
            if (_virtualJoystick != null && _virtualJoystick.Direction.sqrMagnitude > 0.01f)
            {
                _moveInput = _virtualJoystick.Direction;
                return;
            }

            // Keyboard fallback (PC / editor)
            float x = 0f;
            float y = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  x -= 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  y -= 1f;
            }

            _moveInput = new Vector2(x, y);

            // Normalize to prevent diagonal speed boost
            if (_moveInput.sqrMagnitude > 1f)
                _moveInput.Normalize();
        }

        private void FixedUpdate()
        {
            if (_stats == null) return;
            _rb.MovePosition(_rb.position + _moveInput * (_stats.MoveSpeed * Time.fixedDeltaTime));
        }

        public Vector2 GetMoveDirection() => _moveInput;
    }
}
