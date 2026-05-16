using UnityEngine;

public class VRBeltAnchor : MonoBehaviour
{
    [SerializeField] private Transform xrCamera;

    [Header("Placement")]
    [SerializeField] private float heightBelowHead = 0.75f;
    [SerializeField] private float forwardDistance = 0.25f;

    private void LateUpdate()
    {
        if (xrCamera == null) return;

        Vector3 forward = xrCamera.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = transform.parent.forward;

        forward.Normalize();

        Vector3 targetPosition =
            xrCamera.position
            + forward * forwardDistance
            + Vector3.down * heightBelowHead;

        transform.position = targetPosition;
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}