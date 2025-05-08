using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
        public bool Interact { get; private set; }
        public bool DropItem { get; private set; }

        public event Action<int> OnInventoryScrolled;
        public event Action<int> OnSlotSelected;

        private InputAction moveAction;
        private InputAction runAction;
        private InputAction lookAction;
        private InputAction crouchAction;
        private InputAction interactAction;
        private InputAction inventoryScrollAction;
        private InputAction slotSelectAction;
        private InputAction dropItemAction;

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
            interactAction = currentMap.FindAction("Interact");
            inventoryScrollAction = currentMap.FindAction("InventoryScroll");
            slotSelectAction = currentMap.FindAction("SlotSelect");
            dropItemAction = currentMap.FindAction("Drop");

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

            interactAction.started += OnInteract;
            interactAction.canceled += OnInteract;

            inventoryScrollAction.performed += OnInventoryScroll;
            slotSelectAction.performed += OnSlotSelect;

            dropItemAction.performed += OnDropItem;
            dropItemAction.canceled += OnDropItem;
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

            interactAction.started -= OnInteract;
            interactAction.canceled -= OnInteract;

            inventoryScrollAction.performed -= OnInventoryScroll;
            slotSelectAction.performed -= OnSlotSelect;

            dropItemAction.performed -= OnDropItem;
            dropItemAction.canceled -= OnDropItem;
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

        private void OnInteract(InputAction.CallbackContext context)
        {
            Interact = context.ReadValueAsButton();
        }

        private void OnInventoryScroll(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            Vector2 scrollValue = context.ReadValue<Vector2>();

            if (scrollValue.y > 0)
            {
                OnInventoryScrolled?.Invoke(1); // Previous item
            }
            else if (scrollValue.y < 0)
            {
                OnInventoryScrolled?.Invoke(-1); // Next item
            }
        }

        private void OnSlotSelect(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            string key = context.control.name;

            int slotIndex = key switch
            {
                "1" => 0,
                "2" => 1,
                "3" => 2,
                "4" => 3,
                _ => -1
            };

            if (slotIndex >= 0)
                OnSlotSelected?.Invoke(slotIndex);
        }

        private void OnDropItem(InputAction.CallbackContext context)
        {
            DropItem = context.ReadValueAsButton();
        }
    }
}
