using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger zone around the belt. When a grabbable item is released (no longer
/// held) inside the zone, it is collected into the inventory via
/// <see cref="VRInventoryController"/> and the physical object is stored away.
/// </summary>
[RequireComponent(typeof(Collider))]
public class VRBeltCollector : MonoBehaviour
{
    [SerializeField] private VRInventoryController inventory;

    private readonly List<VRGrabbableInventoryItem> inside = new List<VRGrabbableInventoryItem>();

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponentInParent<VRGrabbableInventoryItem>();
        if (item != null && !inside.Contains(item))
            inside.Add(item);
    }

    private void OnTriggerExit(Collider other)
    {
        var item = other.GetComponentInParent<VRGrabbableInventoryItem>();
        if (item != null)
            inside.Remove(item);
    }

    private void Update()
    {
        if (inventory == null || inside.Count == 0) return;

        // Iterate backwards so we can remove collected items safely.
        for (int i = inside.Count - 1; i >= 0; i--)
        {
            var item = inside[i];
            if (item == null) { inside.RemoveAt(i); continue; }

            // Only absorb once the hand has released it inside the zone.
            if (item.IsGrabbed) continue;

            if (inventory.Collect(item))
                inside.RemoveAt(i);
        }
    }
}
