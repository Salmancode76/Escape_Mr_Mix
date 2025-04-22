using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityTutorial.Manager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool Jump { get; private set; }
        public bool Crouch { get; private set; }
        public bool Slide { get; set; }  // This flag triggers a slide

        private InputActionMap currentMap;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;
        private InputAction jumpAction;
        private InputAction crouchAction;
        private InputAction slideAction;

        private void Awake()
        {
            HideCursor();
            currentMap = playerInput.currentActionMap;
            moveAction = currentMap.FindAction("Move");
            lookAction = currentMap.FindAction("Look");
            runAction  = currentMap.FindAction("Run");
            jumpAction = currentMap.FindAction("Jump");
            crouchAction = currentMap.FindAction("Crouch");
            slideAction = currentMap.FindAction("Slid");

            moveAction.performed += OnMove;
            lookAction.performed += OnLook;
            runAction.performed += OnRun;
            jumpAction.performed += OnJump;
            crouchAction.performed += OnCrouchToggle;
            slideAction.performed += OnSlide;

            moveAction.canceled += OnMove;
            lookAction.canceled += OnLook;
            runAction.canceled += OnRun;
            jumpAction.canceled += OnJump;
            // No canceled event for crouch and slide as these are one-shot toggles.
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

        private void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            Jump = context.ReadValueAsButton();
        }

        private void OnCrouchToggle(InputAction.CallbackContext context)
        {
            // Each press toggles the crouch state.
            if (context.performed)
                Crouch = !Crouch;
        }

        private void OnSlide(InputAction.CallbackContext context)
        {
            // Slide triggers on key press.
            if (context.performed)
                Slide = true;
        }

        private void OnEnable()
        {
            currentMap.Enable();
        }

        private void OnDisable()
        {
            currentMap.Disable();
        }
    }
}
