using System.Collections.Generic;
using Flat.Managers;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class VRRigMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager input;
    [SerializeField] private Transform xrCamera;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [Tooltip("How fast the horizontal speed ramps up/down (higher = snappier, lower = floatier).")]
    [SerializeField] private float acceleration = 12f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private float verticalVelocity;
    private Vector3 horizontalVelocity;

    private readonly List<InputDevice> controllers = new List<InputDevice>();

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (input == null || xrCamera == null)
            return;

        Vector2 moveInput = input.Move;
        bool running = input.Run || ThumbstickClicked();
        float speed = running ? runSpeed : walkSpeed;

        Vector3 forward = xrCamera.forward;
        Vector3 right = xrCamera.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Smoothly ramp toward the target velocity for fluid accel/decel.
        Vector3 targetVelocity = (forward * moveInput.y + right * moveInput.x) * speed;
        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity, targetVelocity, acceleration * Time.deltaTime);

        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = horizontalVelocity;
        move.y = verticalVelocity;

        characterController.Move(move * Time.deltaTime);
    }

    /// <summary>True if either controller's thumbstick is clicked (pressed in).</summary>
    private bool ThumbstickClicked()
    {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, controllers);
        foreach (var device in controllers)
        {
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool clicked) && clicked)
                return true;
        }
        return false;
    }
}
