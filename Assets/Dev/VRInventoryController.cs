using System.Collections.Generic;
using Flat.Gameplay.Inventory;
using Flat.Gameplay.Inventory.Items;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// VR belt inventory: grab a world item and drop it on the belt to collect it,
/// then cycle the active item with a button. Stored items stay visible floating
/// at the hip belt slots; the active item is held in the equip hand and can be
/// used with the index trigger.
///
/// Input is read through <see cref="UnityEngine.XR.InputDevices"/> (the OpenXR
/// path the rest of the rig uses), not OVRInput.
///
/// Controls (left controller):
///   - Cycle item : X (primary button)
///   - Use item   : left index trigger
/// Collect: grab an item with either hand and release it on the belt.
/// </summary>
public class VRInventoryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [Tooltip("Where the active item is held (e.g. LeftHandAnchor).")]
    [SerializeField] private Transform equipAnchor;
    [Tooltip("Hip anchors where stored items float (one per slot).")]
    [SerializeField] private Transform[] beltSlots = new Transform[SLOTS];

    [Header("Belt display")]
    [Tooltip("Local rotation applied to items resting on the belt.")]
    [SerializeField] private Vector3 beltEuler = Vector3.zero;

    private const int SLOTS = 4;
    private readonly GameObject[] stored = new GameObject[SLOTS];
    private int equippedSlot = -1;

    private InputDevice leftHand;
    private InputDevice rightHand;
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
            grabItem.IsCollected = true;

            Debug.Log($"[VRInventory] Collected {grabItem.gameObject.name} into slot {i}");

            if (equippedSlot < 0)
                Equip(i);          // first item goes straight to the hand
            else
                PlaceOnBelt(i);    // others float on the belt

            return true;
        }
        return false; // inventory full
    }

    private void Update()
    {
        EnsureDevices();

        bool cycle = ReadButton(leftHand, CommonUsages.primaryButton) || 
                     ReadButton(rightHand, CommonUsages.primaryButton);   // X or A
        
        bool use = ReadButton(leftHand, CommonUsages.triggerButton) ||
                   ReadButton(rightHand, CommonUsages.triggerButton);     // either trigger

        if (cycle && !prevCycle)
        {
            Debug.Log($"[VRInventory] Cycle input detected. Current equipped: {equippedSlot}");
            CycleNext();
        }
        if (use && !prevUse) UsePressed();

        prevCycle = cycle;
        prevUse = use;
    }

    private void EnsureDevices()
    {
        if (!leftHand.isValid)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, devices);
            if (devices.Count > 0) leftHand = devices[0];
        }
        if (!rightHand.isValid)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, devices);
            if (devices.Count > 0) rightHand = devices[0];
        }
    }

    private bool ReadButton(InputDevice device, InputFeatureUsage<bool> usage)
    {
        return device.isValid && device.TryGetFeatureValue(usage, out bool pressed) && pressed;
    }

    private void CycleNext()
    {
        if (inventory == null) return;

        for (int step = 1; step <= SLOTS; step++)
        {
            int idx = (((equippedSlot < 0 ? -1 : equippedSlot) + step) % SLOTS + SLOTS) % SLOTS;
            
            // Only cycle to slots that both have an item in inventory AND a tracked GameObject
            if (!inventory.IsSlotEmpty(idx) && stored[idx] != null)
            {
                Debug.Log($"[VRInventory] Found valid item at slot {idx}. Equipping.");
                Equip(idx);
                return;
            }
        }
        Debug.Log("[VRInventory] No other valid items to cycle to.");
    }

    private void Equip(int slot)
    {
        if (slot == equippedSlot) return;

        var obj = stored[slot];
        if (obj == null) return;

        // Send the previously held item back to its belt slot.
        if (equippedSlot >= 0 && equippedSlot < SLOTS)
            PlaceOnBelt(equippedSlot);

        obj.SetActive(true);
        SetInteractable(obj, false);

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Disable colliders to prevent "lifting" physics issues with the player
        foreach (var col in obj.GetComponentsInChildren<Collider>(true))
            col.enabled = false;

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

    /// <summary>Park a stored item floating at its hip belt slot (still visible).</summary>
    private void PlaceOnBelt(int slot)
    {
        var obj = stored[slot];
        if (obj == null) return;

        obj.SetActive(true);
        SetInteractable(obj, false);

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (var col in obj.GetComponentsInChildren<Collider>(true))
            col.enabled = false;

        Transform anchor = (beltSlots != null && slot < beltSlots.Length) ? beltSlots[slot] : null;
        if (anchor != null)
        {
            obj.transform.SetParent(anchor, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(beltEuler);
        }
    }

    private void UsePressed()
    {
        if (inventory == null) return;

        // Use the item currently held in hand (flashlight toggle, etc.).
        if (equippedSlot >= 0)
        {
            var equipped = inventory.GetItemAt(equippedSlot);
            if (equipped != null)
            {
                equipped.Use(stored[equippedSlot]);
                inventory.OnItemUsed(equipped);
            }
        }

        // A key stored on the belt is usable without equipping: it opens a
        // nearby door from where it sits on the hip.
        for (int i = 0; i < SLOTS; i++)
        {
            if (i == equippedSlot) continue;
            if (inventory.GetItemAt(i) is KeyItem keyItem)
                keyItem.Use(stored[i] != null ? stored[i] : gameObject);
        }
    }

    /// <summary>Enable/disable the Meta grab components so a stored/equipped item can't be grabbed.</summary>
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
