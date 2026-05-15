using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VRInteractorRayVisual : MonoBehaviour
{
    [SerializeField] private float length = 4f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.widthMultiplier = 0.01f;
    }

    private void LateUpdate()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + transform.forward * length);
    }
}