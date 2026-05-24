using Flat.Gameplay.Interaction;
using UnityEngine;

namespace Flat.Gameplay.Inventory
{
    public class ItemPickup : BaseInteractable
    {
        [SerializeField] private Item item;

        [Header("References")]
        [SerializeField] private GameObject player;

        private PlayerInventory playerInventory;
        
        private void Start()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                playerInventory = player.GetComponentInChildren<PlayerInventory>();
        }

        public override void Interact()
        {
            if (playerInventory == null)
            {
                // Last ditch effort if not found in Start
                playerInventory = Object.FindAnyObjectByType<PlayerInventory>();
            }

            if (playerInventory == null) return;

            if (playerInventory.AddItem(item))
            {
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Inventory is full!");
            }
        }

        public void SetPlayer(GameObject newPlayer)
        {
            player = newPlayer;
            playerInventory = player.GetComponentInChildren<PlayerInventory>();
        }
}
}
