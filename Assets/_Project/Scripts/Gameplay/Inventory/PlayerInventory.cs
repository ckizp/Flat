using System.Collections.Generic;
using System;
using UnityEngine;

namespace Flat.Gameplay.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        private const int SLOTS = 4;
        private Item[] items;
        private int selectedSlotIndex = 0;

        public event EventHandler<InventoryEventArgs> ItemAdded;
        public event EventHandler<InventoryEventArgs> ItemRemoved;
        public event EventHandler<InventoryEventArgs> ItemSelected;
        public event EventHandler<InventoryEventArgs> ItemUsed;

        private void Awake()
        {
            items = new Item[SLOTS];
        }

        private void Start()
        {
            TriggerSelectionEvent();
        }

        private void TriggerSelectionEvent()
        {
            Item selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                ItemSelected?.Invoke(this, new InventoryEventArgs(selectedItem));
            }
            else
            {
                ItemSelected?.Invoke(this, new InventoryEventArgs(null));
            }
        }

        public bool AddItem(Item item)
        {
            for (int i = 0; i < SLOTS; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    ItemAdded?.Invoke(this, new InventoryEventArgs(item));

                    if (GetSelectedItem() == null)
                    {
                        selectedSlotIndex = i;
                        TriggerSelectionEvent();
                    }

                    return true;
                }
            }

            return false;
        }

        public void RemoveItemAt(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < SLOTS && items[slotIndex] != null)
            {
                Item removedItem = items[slotIndex];
                items[slotIndex] = null;

                ItemRemoved?.Invoke(this, new InventoryEventArgs(removedItem));

                if (slotIndex == selectedSlotIndex)
                {
                    TriggerSelectionEvent();
                }
            }
        }

        public void SelectSlot(int index)
        {
            if (index >= 0 && index < SLOTS)
            {
                selectedSlotIndex = index;
                TriggerSelectionEvent();
            }
        }

        public void SelectNextSlot()
        {
            selectedSlotIndex = (selectedSlotIndex + 1) % SLOTS;
            TriggerSelectionEvent();
        }

        public void SelectPreviousSlot()
        {
            selectedSlotIndex = (selectedSlotIndex - 1 + SLOTS) % SLOTS;
            TriggerSelectionEvent();
        }

        public void DropSelectedItem()
        {
            Item selectedItem = GetSelectedItem();

            if (selectedItem != null && selectedItem.prefab != null)
            {
                Camera playerCamera = Camera.main;
                if (playerCamera != null)
                {
                    Vector3 dropPosition = playerCamera.transform.position + playerCamera.transform.forward * 1.5f;
                    GameObject droppedObj = Instantiate(selectedItem.prefab, dropPosition, Quaternion.identity);

                    ItemPickup pickup = droppedObj.GetComponent<ItemPickup>();
                    if (pickup != null)
                    {
                        pickup.SetPlayer(gameObject);
                    }
                }

                RemoveItemAt(selectedSlotIndex);
            }
        }

        public Item GetSelectedItem()
        {
            if (selectedSlotIndex >= 0 && selectedSlotIndex < SLOTS)
            {
                return items[selectedSlotIndex];
            }
            return null;
        }

        public int GetSelectedIndex()
        {
            return selectedSlotIndex;
        }

        public Item GetItemAt(int index)
        {
            if (index >= 0 && index < SLOTS)
            {
                return items[index];
            }
            return null;
        }
        
        public void OnItemUsed(Item item)
        {
            ItemUsed?.Invoke(this, new InventoryEventArgs(item));
        }

        public bool HasItem(string itemName)
        {
            for (int i = 0; i < SLOTS; i++)
            {
                if (items[i] != null && items[i].itemName == itemName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}