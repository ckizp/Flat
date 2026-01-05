using System;
using UnityEngine;

namespace Flat.Gameplay.Inventory.Items
{
    [CreateAssetMenu(fileName = "New TV Remote", menuName = "Flat/Inventory/TV Remote")]
    public class TvRemoteItem : Item
    {
        public static event Action OnRemoteUsed;

        public override void Use()
        {
            OnRemoteUsed?.Invoke();
        }

        public override void Use(GameObject heldItemInstance)
        {
            Use();
        }
    }
}