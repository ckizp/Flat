using UnityEngine;
using UnityEngine.InputSystem;

namespace Flat.Managers
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        private PlayerInput playerInput;

        public Vector2 Move { get; private set; }
        public bool Run { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Crouch { get; private set; }

        private InputAction moveAction;
        private InputAction runAction;
        private InputAction lookAction;
        private InputAction crouchAction;

        private void Awake()
        {
            HideCursor();

            playerInput = GetComponent<PlayerInput>();

            // Retrieve input actions from the current action map
            var currentMap = playerInput.currentActionMap;

            moveAction = currentMap.FindAction("Move");
            runAction = currentMap.FindAction("Sprint");
            lookAction = currentMap.FindAction("Look");
            crouchAction = currentMap.FindAction("Crouch");

            RegisterInputCallbacks();
        }

        private void OnEnable()
        {
            playerInput.currentActionMap?.Enable();
        }

        private void OnDisable()
        {
            UnregisterInputCallbacks();
            playerInput.currentActionMap?.Disable();
        }

        private void RegisterInputCallbacks()
        {
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            runAction.performed += OnRun;
            runAction.canceled += OnRun;

            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;

            crouchAction.started += OnCrouch;
            crouchAction.canceled += OnCrouch;
        }

        private void UnregisterInputCallbacks()
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;

            runAction.performed -= OnRun;
            runAction.canceled -= OnRun;

            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;

            crouchAction.started -= OnCrouch;
            crouchAction.canceled -= OnCrouch;
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        private void OnCrouch(InputAction.CallbackContext context)
        {
            Crouch = context.ReadValueAsButton();
        }
    }
}
