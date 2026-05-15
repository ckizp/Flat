using UnityEngine;
using UnityEngine.InputSystem;

public class XRHandTransformDriver : MonoBehaviour
{
    [SerializeField] private InputActionReference positionAction;
    [SerializeField] private InputActionReference rotationAction;

    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    private void OnEnable()
    {
        positionAction?.action.Enable();
        rotationAction?.action.Enable();
    }

    private void OnDisable()
    {
        positionAction?.action.Disable();
        rotationAction?.action.Disable();
    }

    private void LateUpdate()
    {
        if (positionAction != null)
            transform.localPosition = positionAction.action.ReadValue<Vector3>() + positionOffset;

        if (rotationAction != null)
            transform.localRotation = rotationAction.action.ReadValue<Quaternion>() * Quaternion.Euler(rotationOffset);
    }
}