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

    private readonly Dictionary<VRGrabbableInventoryItem, int> insideCount = new Dictionary<VRGrabbableInventoryItem, int>();
    private readonly List<VRGrabbableInventoryItem> inside = new List<VRGrabbableInventoryItem>(); // Kept for iteration

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponentInParent<VRGrabbableInventoryItem>();
        if (item == null) return;

        if (!insideCount.ContainsKey(item))
        {
            insideCount[item] = 1;
            inside.Add(item);
            Debug.Log($"[VRBelt] Item {item.gameObject.name} entered belt zone.");
        }
        else
        {
            insideCount[item]++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var item = other.GetComponentInParent<VRGrabbableInventoryItem>();
        if (item == null) return;

        if (insideCount.ContainsKey(item))
        {
            insideCount[item]--;
            if (insideCount[item] <= 0)
            {
                insideCount.Remove(item);
                inside.Remove(item);
                Debug.Log($"[VRBelt] Item {item.gameObject.name} exited belt zone.");
            }
        }
    }

    private void Update()
    {
        if (inventory == null || inside.Count == 0) return;

        for (int i = inside.Count - 1; i >= 0; i--)
        {
            var item = inside[i];
            if (item == null)
            {
                inside.RemoveAt(i);
                continue;
            }

            if (item.IsCollected)
            {
                inside.RemoveAt(i);
                insideCount.Remove(item);
                continue;
            }

            // Only absorb once the hand has released it inside the zone.
            if (item.IsGrabbed) continue;

            Debug.Log($"[VRBelt] Attempting to collect {item.gameObject.name}...");
            if (inventory.Collect(item))
            {
                Debug.Log($"[VRBelt] Successfully collected {item.gameObject.name}.");
                inside.RemoveAt(i);
                insideCount.Remove(item);
            }
        }
    }
}
