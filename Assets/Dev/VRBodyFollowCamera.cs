using UnityEngine;

public class VRBodyFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform xrCamera;
    [SerializeField] private bool followYaw = true;

    private void LateUpdate()
    {
        if (xrCamera == null) return;

        Vector3 camLocalPos = xrCamera.localPosition;

        transform.localPosition = new Vector3(
            camLocalPos.x,
            0f,
            camLocalPos.z
        );

        if (followYaw)
        {
            Vector3 camForward = xrCamera.forward;
            camForward.y = 0f;

            if (camForward.sqrMagnitude > 0.001f)
            {
                Quaternion worldYaw = Quaternion.LookRotation(camForward, Vector3.up);
                transform.rotation = worldYaw;
            }
        }
    }
}