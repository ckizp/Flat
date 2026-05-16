using Flat.Gameplay.Inventory;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class VRGrabbableInventoryItem : MonoBehaviour
{
    [SerializeField] private Item item;

    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private VRBeltSlot currentSlot;

    public Item Item => item;
    public bool IsInBelt => currentSlot != null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (currentSlot != null)
        {
            currentSlot.RemoveItemFromSlot(this);
            currentSlot = null;
        }

        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void SnapToSlot(VRBeltSlot slot, Transform slotAnchor)
    {
        currentSlot = slot;

        rb.isKinematic = true;
        rb.useGravity = false;

        transform.SetParent(slotAnchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void DetachFromSlot()
    {
        currentSlot = null;

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
    }
}