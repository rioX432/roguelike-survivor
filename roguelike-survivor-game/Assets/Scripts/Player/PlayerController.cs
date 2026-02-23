using UnityEngine;
using UnityEngine.InputSystem;

namespace RoguelikeSurvivor
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStats _stats;
        [SerializeField] private VirtualJoystick _virtualJoystick;

        private Rigidbody2D _rb;
        private InputSystem_Actions _inputActions;
        private Vector2 _moveInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void Update()
        {
            // Read keyboard/gamepad input from InputSystem
            Vector2 keyboardInput = _inputActions.Player.Move.ReadValue<Vector2>();

            // Prefer virtual joystick on mobile; fall back to keyboard
            if (_virtualJoystick != null && _virtualJoystick.Direction.sqrMagnitude > 0.01f)
            {
                _moveInput = _virtualJoystick.Direction;
            }
            else
            {
                _moveInput = keyboardInput;
            }

            // Normalize to prevent diagonal speed boost
            if (_moveInput.sqrMagnitude > 1f)
            {
                _moveInput.Normalize();
            }
        }

        private void FixedUpdate()
        {
            if (_stats == null) return;

            Vector2 newPosition = _rb.position + _moveInput * (_stats.MoveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);
        }
    }
}
