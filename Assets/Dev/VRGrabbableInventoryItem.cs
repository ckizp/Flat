using Flat.Gameplay.Inventory;
using Oculus.Interaction;
using UnityEngine;

/// <summary>
/// Marks a Meta-grabbable world object as collectable into the belt inventory.
/// Only tracks the linked <see cref="Item"/> and whether a hand is currently
/// holding it (via the Meta <see cref="Grabbable"/> events). The actual
/// collect / equip / use flow is handled by <see cref="VRInventoryController"/>.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VRGrabbableInventoryItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [Tooltip("Meta Grabbable driving this item. Auto-found if left empty.")]
    [SerializeField] private Grabbable grabbable;

    [Header("Equip pose (how it sits in the inventory hand)")]
    [SerializeField] private Vector3 equipPosition = new Vector3(0f, 0f, 0.05f);
    [SerializeField] private Vector3 equipEuler = Vector3.zero;

    private bool isGrabbed;

    public Item Item => item;
    public bool IsGrabbed => isGrabbed;
    /// <summary>Set once the item is taken into the belt inventory, so it is not re-collected.</summary>
    public bool IsCollected { get; set; }
    public Vector3 EquipPosition => equipPosition;
    public Vector3 EquipEuler => equipEuler;

    private void Awake()
    {
        if (grabbable == null)
            grabbable = GetComponentInChildren<Grabbable>();
    }

    private void OnEnable()
    {
        if (grabbable != null)
            grabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void OnDisable()
    {
        if (grabbable != null)
            grabbable.WhenPointerEventRaised -= HandlePointerEvent;
        isGrabbed = false;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                isGrabbed = true;
                break;
            case PointerEventType.Unselect:
            case PointerEventType.Cancel:
                isGrabbed = false;
                break;
        }
    }
}
