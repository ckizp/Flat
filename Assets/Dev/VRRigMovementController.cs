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

    [Header("Turning (right stick, snap = comfort)")]
    [Tooltip("Degrees per snap turn.")]
    [SerializeField] private float snapAngle = 30f;
    [Tooltip("Stick push needed to trigger a snap turn.")]
    [SerializeField] private float turnThreshold = 0.7f;

    private CharacterController characterController;
    private float verticalVelocity;
    private Vector3 horizontalVelocity;
    private bool turnReady = true;

    private readonly List<InputDevice> controllers = new List<InputDevice>();
    private readonly List<InputDevice> rightControllers = new List<InputDevice>();
    private readonly List<InputDevice> leftControllers = new List<InputDevice>();

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        HandleSnapTurn();
    }

    /// <summary>Snap-rotates the rig with the right thumbstick (comfort turning).</summary>
    private void HandleSnapTurn()
    {
        float x = 0f;
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, rightControllers);
        foreach (var device in rightControllers)
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
                x = axis.x;

        if (turnReady && Mathf.Abs(x) > turnThreshold)
        {
            float angle = Mathf.Sign(x) * snapAngle;
            Vector3 pivot = xrCamera != null ? xrCamera.position : transform.position;
            transform.RotateAround(pivot, Vector3.up, angle);
            turnReady = false;
        }
        else if (Mathf.Abs(x) < 0.3f)
        {
            turnReady = true;
        }
    }

    private void Move()
    {
        if (xrCamera == null)
            return;

        // Movement is read directly from the LEFT stick so it never conflicts with
        // the RIGHT stick used for turning.
        Vector2 moveInput = ReadLeftThumbstick();
        bool running = (input != null && input.Run) || ThumbstickClicked();
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

    /// <summary>Reads the left controller's thumbstick (movement axis).</summary>
    private Vector2 ReadLeftThumbstick()
    {
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, leftControllers);
        foreach (var device in leftControllers)
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
                return axis;
        return Vector2.zero;
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
