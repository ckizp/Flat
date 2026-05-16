using Flat.Gameplay.Inventory;
using UnityEngine;

public class VRBeltSlot : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] private int slotIndex;
    [SerializeField] private Transform snapAnchor;

    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    private VRGrabbableInventoryItem currentItem;
    private VRGrabbableInventoryItem nearbyItem;

    private void Awake()
    {
        if (snapAnchor == null)
            snapAnchor = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        VRGrabbableInventoryItem item = other.GetComponentInParent<VRGrabbableInventoryItem>();

        if (item == null)
            return;

        nearbyItem = item;
    }

    private void OnTriggerExit(Collider other)
    {
        VRGrabbableInventoryItem item = other.GetComponentInParent<VRGrabbableInventoryItem>();

        if (item == null)
            return;

        if (nearbyItem == item)
            nearbyItem = null;
    }

    private void Update()
    {
        if (currentItem != null)
            return;

        if (nearbyItem == null)
            return;

        if (playerInventory == null)
            return;

        if (!playerInventory.IsSlotEmpty(slotIndex))
            return;

        Rigidbody rb = nearbyItem.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        // Simple rule: si l'objet est proche du slot et presque immobile, on le range.
        if (rb.linearVelocity.sqrMagnitude > 0.05f)
            return;

        StoreNearbyItem();
    }

    private void StoreNearbyItem()
    {
        if (nearbyItem == null)
            return;

        if (nearbyItem.Item == null)
            return;

        bool added = playerInventory.AddItemAt(slotIndex, nearbyItem.Item);

        if (!added)
            return;

        currentItem = nearbyItem;
        currentItem.SnapToSlot(this, snapAnchor);
        nearbyItem = null;
    }

    public void RemoveItemFromSlot(VRGrabbableInventoryItem item)
    {
        if (currentItem != item)
            return;

        playerInventory.RemoveItemAt(slotIndex);

        currentItem.DetachFromSlot();
        currentItem = null;
    }
}