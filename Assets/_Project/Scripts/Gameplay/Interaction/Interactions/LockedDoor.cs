using System.Collections;
using Flat.Gameplay.Inventory;
using UnityEngine;

namespace Flat.Gameplay.Interaction.Interactions
{
    public class LockedDoor : BaseInteractable
    {
        [Header("Door Properties")]
        [SerializeField] private Animator animator;
        [SerializeField] private bool isLocked = true;
        [SerializeField] private string keyItemName = "Key";

        private bool isOpen = false;

        public override void Interact()
        {
            if (isLocked)
            {
                TriggerInteraction("try_open_entrance_door");

                // The key only needs to be in the inventory (e.g. stored on the belt).
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    PlayerInventory inventory = player.GetComponentInChildren<PlayerInventory>();
                    if (inventory != null && inventory.HasItem(keyItemName))
                    {
                        isLocked = false;
                        StartCoroutine(Open());
                    }
                }
            }
else if (!isOpen)
            {
                StartCoroutine(Open());
            }
            else
            {
                StartCoroutine(Close());
            }
        }

        /// <summary>
        /// Called when the player uses a key item. Unlocks and opens the door only
        /// if the key matches. Returns true if this door accepted the key.
        /// </summary>
        public bool TryUnlockWithKey(string keyName)
        {
            if (isLocked && keyName != keyItemName)
                return false;

            if (isLocked)
                isLocked = false;

            if (!isOpen)
                StartCoroutine(Open());

            return true;
        }

        private IEnumerator Open()
        {
            animator.Play("Opening");
            isOpen = true;
            TriggerInteraction("entrance_door_open");
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator Close()
        {
            animator.Play("Closing");
            isOpen = false;
            yield return new WaitForSeconds(0.5f);
        }

        public void Unlock()
        {
            isLocked = false;
        }
    }
}
