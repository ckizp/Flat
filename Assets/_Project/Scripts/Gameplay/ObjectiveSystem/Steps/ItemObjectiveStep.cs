using Flat.Gameplay.Inventory;
using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem.Steps
{
    public class ItemObjectiveStep : ObjectiveStep
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
                    // Check if player already has the item
                    if (inventory.HasItem(targetItemName))
                    {
                        Debug.Log($"[ItemObjective] Player already has {targetItemName}. Completing step.");
                        FinishObjectiveStep();
                        return;
                    }

                    inventory.ItemAdded += OnItemAdded;
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
                    inventory.ItemAdded -= OnItemAdded;
                }
            }
        }

        private void OnItemAdded(object sender, InventoryEventArgs e)
        {
            if (e.Item.itemName == targetItemName)
            {
                FinishObjectiveStep();
            }
        }
    }
}