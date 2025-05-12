using System.Collections;
using Flat.Gameplay.Interaction;
using Flat.Gameplay.Interaction.Implementations;
using Flat.Gameplay.Inventory;
using UnityEngine;

namespace Flat.Gameplay.Interaction.Implementations
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

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    PlayerInventory inventory = player.GetComponent<PlayerInventory>();
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
    }
}
