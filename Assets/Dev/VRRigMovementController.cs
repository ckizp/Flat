using Flat.Managers;
using UnityEngine;

public class VRRigMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager input;
    [SerializeField] private Transform xrCamera;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 3.5f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (input == null || xrCamera == null) return;

        Vector2 moveInput = input.Move;
        float speed = input.Run ? runSpeed : walkSpeed;

        Vector3 forward = xrCamera.forward;
        Vector3 right = xrCamera.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        transform.position += move * speed * Time.deltaTime;
    }
}