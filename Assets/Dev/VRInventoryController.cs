using System.Collections.Generic;
using Flat.Gameplay.Inventory;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// VR belt inventory: grab a world item and drop it on the belt to collect it,
/// then cycle the active item with a button. The active item is shown held in
/// the equip hand and can be used with the index trigger.
///
/// Input is read through <see cref="UnityEngine.XR.InputDevices"/> (the OpenXR
/// path that the rest of the rig already uses), not OVRInput.
///
/// Controls (left controller):
///   - Cycle item : X (primary button)
///   - Use item   : left index trigger
///   - Drop item  : Y (secondary button)
/// Collect: grab an item with either hand and release it on the belt.
/// </summary>
public class VRInventoryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [Tooltip("Where the active item is held (e.g. LeftHandAnchor).")]
    [SerializeField] private Transform equipAnchor;

    private const int SLOTS = 4;
    private readonly GameObject[] stored = new GameObject[SLOTS];
    private int equippedSlot = -1;

    // Left controller + previous button states for edge detection.
    private InputDevice leftHand;
    private bool prevCycle, prevUse;

    /// <summary>Collect a grabbed item into the first free slot. Returns false if full.</summary>
    public bool Collect(VRGrabbableInventoryItem grabItem)
    {
        if (grabItem == null || grabItem.Item == null || inventory == null) return false;

        for (int i = 0; i < SLOTS; i++)
        {
            if (!inventory.IsSlotEmpty(i)) continue;

            inventory.AddItemAt(i, grabItem.Item);
            stored[i] = grabItem.gameObject;
            grabItem.gameObject.transform.SetParent(null);
            grabItem.gameObject.SetActive(false);

            if (equippedSlot < 0)
                Equip(i);

            return true;
        }
        return false; // inventory full
    }

    private void Update()
    {
        EnsureLeftHand();

        bool cycle = ReadButton(CommonUsages.primaryButton);   // X
        bool use = ReadButton(CommonUsages.triggerButton);     // index

        if (cycle && !prevCycle) CycleNext();
        if (use && !prevUse) UseEquipped();

        prevCycle = cycle;
        prevUse = use;
    }

    private void EnsureLeftHand()
    {
        if (leftHand.isValid) return;

        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, devices);
        if (devices.Count > 0)
            leftHand = devices[0];
    }

    private bool ReadButton(InputFeatureUsage<bool> usage)
    {
        return leftHand.isValid && leftHand.TryGetFeatureValue(usage, out bool pressed) && pressed;
    }

    private void CycleNext()
    {
        if (inventory == null) return;

        for (int step = 1; step <= SLOTS; step++)
        {
            int idx = (((equippedSlot < 0 ? -1 : equippedSlot) + step) % SLOTS + SLOTS) % SLOTS;
            if (!inventory.IsSlotEmpty(idx))
            {
                Equip(idx);
                return;
            }
        }
    }

    private void Equip(int slot)
    {
        if (slot == equippedSlot) return;

        Unequip();

        var obj = stored[slot];
        if (obj == null) return;

        obj.SetActive(true);
        SetInteractable(obj, false);

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

        if (equipAnchor != null)
        {
            obj.transform.SetParent(equipAnchor, false);

            Vector3 pos = new Vector3(0f, 0f, 0.05f);
            Vector3 euler = Vector3.zero;
            var grabItem = obj.GetComponent<VRGrabbableInventoryItem>();
            if (grabItem != null)
            {
                pos = grabItem.EquipPosition;
                euler = grabItem.EquipEuler;
            }
            obj.transform.localPosition = pos;
            obj.transform.localRotation = Quaternion.Euler(euler);
        }

        equippedSlot = slot;
        inventory.SelectSlot(slot);
    }

    private void Unequip()
    {
        if (equippedSlot < 0) return;

        var obj = stored[equippedSlot];
        if (obj != null)
        {
            obj.transform.SetParent(null);
            obj.SetActive(false);
        }
        equippedSlot = -1;
    }

    private void UseEquipped()
    {
        if (equippedSlot < 0 || inventory == null) return;

        var item = inventory.GetItemAt(equippedSlot);
        if (item == null) return;

        item.Use(stored[equippedSlot]);
        inventory.OnItemUsed(item);
    }

    /// <summary>Enable/disable the Meta grab components so an equipped item can't be grabbed.</summary>
    private static void SetInteractable(GameObject obj, bool enabled)
    {
        foreach (var mb in obj.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb == null) continue;
            string n = mb.GetType().Name;
            if (n == "Grabbable" || n.Contains("HandGrabInteractable") || n.Contains("GrabInteractable"))
                mb.enabled = enabled;
        }
    }
}
