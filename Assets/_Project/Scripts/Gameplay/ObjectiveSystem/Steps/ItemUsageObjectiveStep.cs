using Flat.Gameplay.Inventory;
using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem.Steps
{
    public class ItemUsageObjectiveStep : ObjectiveStep
    {
        [SerializeField] private string targetItemName;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerInventory inventory = player.GetComponentInChildren<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.ItemUsed += OnItemUsed;
                }
            }
        }

        private void OnDestroy()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerInventory inventory = player.GetComponentInChildren<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.ItemUsed -= OnItemUsed;
                }
            }
        }

        private void OnItemUsed(object sender, InventoryEventArgs e)
        {
            if (e.Item.itemName == targetItemName)
            {
                FinishObjectiveStep();
            }
        }
    }
}
