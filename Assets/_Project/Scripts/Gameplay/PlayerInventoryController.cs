using System.Collections;
using Flat.Gameplay.Inventory;
using Flat.Managers;
using UnityEngine;

namespace Flat.Gameplay.Characters
{
    [RequireComponent(typeof(InputManager), typeof(PlayerInventory))]
    public class PlayerInventoryController : MonoBehaviour
    {
        private InputManager inputManager;
        private PlayerInventory inventory;
        private bool scrollCooldown = false;

        private void Awake()
        {
            inputManager = GetComponent<InputManager>();
            inventory = GetComponent<PlayerInventory>();

            inputManager.OnInventoryScrolled += HandleInventoryScroll;

            inputManager.OnSlotSelected += HandleSlotSelect;
        }

        private void OnDestroy()
        {
            if (inputManager != null)
            {
                inputManager.OnInventoryScrolled -= HandleInventoryScroll;
                inputManager.OnSlotSelected -= HandleSlotSelect;
            }
        }

        private void Update()
        {
            if (inputManager.DropItem)
            {
                inventory.DropSelectedItem();
            }
        }

        private void HandleInventoryScroll(int direction)
        {
            if (scrollCooldown)
                return;

            if (direction > 0)
            {
                inventory.SelectNextSlot();
                StartCoroutine(ScrollCooldownRoutine());
            }
            else
            {
                inventory.SelectPreviousSlot();
                StartCoroutine(ScrollCooldownRoutine());
            }
        }

        private void HandleSlotSelect(int slotIndex)
        {
            inventory.SelectSlot(slotIndex);
        }

        private IEnumerator ScrollCooldownRoutine()
        {
            scrollCooldown = true;
            yield return new WaitForSeconds(0.2f);
            scrollCooldown = false;
        }
    }
}