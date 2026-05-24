using System.Linq;
using Flat.Gameplay.Interaction.Interactions;
using UnityEngine;

namespace Flat.Gameplay.Inventory.Items
{
    /// <summary>
    /// A key that, when used while held, opens the nearest locked door within range.
    /// The door itself still verifies the player owns the matching key.
    /// </summary>
    [CreateAssetMenu(fileName = "New Key", menuName = "Flat/Inventory/Key")]
    public class KeyItem : Item
    {
        [SerializeField] private float useRange = 3f;

        public override void Use(GameObject heldItemInstance)
        {
            if (heldItemInstance == null) return;

            Vector3 origin = heldItemInstance.transform.position;

            // Open the matching locked door within range (closest first).
            foreach (var door in Object.FindObjectsByType<LockedDoor>(FindObjectsSortMode.None)
                         .OrderBy(d => Vector3.Distance(origin, d.transform.position)))
            {
                if (Vector3.Distance(origin, door.transform.position) > useRange)
                    break;

                if (door.TryUnlockWithKey(itemName))
                    return;
            }
        }
    }
}
