using System;
using UnityEngine;

namespace Flat.Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Flat/Inventory/Item")]
    public class Item : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public GameObject prefab;

        public virtual void Use() { }
        
        public virtual void Use(GameObject heldItemInstance)
        {
            Use();
        }
    }

    public class InventoryEventArgs : EventArgs
    {
        public InventoryEventArgs(Item item)
        {
            Item = item;
        }

        public Item Item;
    }
}
