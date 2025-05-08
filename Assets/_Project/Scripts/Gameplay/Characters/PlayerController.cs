using Flat.Managers;
using UnityEngine;

namespace Flat.Gameplay.Characters
{
    [RequireComponent(typeof(Rigidbody), typeof(InputManager), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float animBlendSpeed = 8.9f;
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float runSpeed = 6f;

        [Header("Camera Settings")]
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private Transform cameraTransform;
        [SerializeField, Tooltip("Vertical camera rotation upper limit (degrees)")]
        private float upperLimit = -40f;
        [SerializeField, Tooltip("Vertical camera rotation bottom limit (degrees)")]
        private float bottomLimit = 70f;
        [SerializeField, Tooltip("Mouse sensitivity multiplier")]
        private float mouseSensitivity = 21.9f;

        private Rigidbody playerRigidBody;
        private InputManager inputManager;
        private Animator animator;

        private static readonly int xVelHash = Animator.StringToHash("X_Velocity");
        private static readonly int yVelHash = Animator.StringToHash("Y_Velocity");
        private static readonly int crouchHash = Animator.StringToHash("Crouch");

        private float xRotation;
        private Vector2 currentVelocity;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerRigidBody = GetComponent<Rigidbody>();
            inputManager = GetComponent<InputManager>();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleCrouch();
        }

        private void LateUpdate()
        {
            HandleCamera();
        }

        private void HandleMovement()
        {
            float targetSpeed = inputManager.Run ? runSpeed : walkSpeed;
            if (inputManager.Crouch) targetSpeed = 1.5f;
            if (inputManager.Move == Vector2.zero) targetSpeed = 0.1f;

            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputManager.Move.x * targetSpeed, animBlendSpeed * Time.fixedDeltaTime);
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, inputManager.Move.y * targetSpeed, animBlendSpeed * Time.fixedDeltaTime);

            var xVelDifference = currentVelocity.x - playerRigidBody.linearVelocity.x;
            var zVelDifference = currentVelocity.y - playerRigidBody.linearVelocity.z;

            playerRigidBody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);

            animator.SetFloat(xVelHash, currentVelocity.x);
            animator.SetFloat(yVelHash, currentVelocity.y);
        }

        private void HandleCamera()
        {
            var Mouse_X = inputManager.Look.x;
            var Mouse_Y = inputManager.Look.y;
            cameraTransform.position = cameraRoot.position;

            xRotation -= Mouse_Y * mouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, upperLimit, bottomLimit);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.Rotate(Vector3.up, Mouse_X * mouseSensitivity * Time.deltaTime);
        }

        private void HandleCrouch() => animator.SetBool(crouchHash, inputManager.Crouch);
    }
}