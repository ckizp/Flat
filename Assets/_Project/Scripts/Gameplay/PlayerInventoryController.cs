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
        private GameObject currentHeldItem;
        [SerializeField] private Transform handAnchor;

        private void Awake()
        {
            BoxCollider test = new BoxCollider();
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

        private void OnEnable()
        {
            inventory.ItemSelected += OnItemSelected;
        }

        private void OnDisable()
        {
            inventory.ItemSelected -= OnItemSelected;
        }

        private void Update()
        {
            if (inputManager.DropItem)
            {
                inventory.DropSelectedItem();
            }

            if (inputManager.UseItem)
            {
                UseSelectedItem();
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
        }        private void UseSelectedItem()
        {
            Item selectedItem = inventory.GetSelectedItem();
            if (selectedItem != null)
            {
                // Vérifier si l'objet est bien instancié, sinon l'instancier
                if (currentHeldItem == null && selectedItem.prefab != null)
                {
                    Debug.Log("Item was not instantiated yet, displaying it now");
                    DisplaySelectedItemInHand();
                }
                
                // Passer l'objet tenu en main à la méthode Use
                selectedItem.Use(currentHeldItem);
            }
        }

        private void DisplaySelectedItemInHand()
        {
            // Destroy the currently held item if it exists
            if (currentHeldItem != null)
            {
                Destroy(currentHeldItem);
            }

            // Get the selected item
            Item selectedItem = inventory.GetSelectedItem();
            if (selectedItem != null && selectedItem.prefab != null)
            {
                // Instantiate the item's prefab and attach it to the hand anchor
                currentHeldItem = Instantiate(selectedItem.prefab, handAnchor);
                currentHeldItem.transform.localPosition = Vector3.zero;
                currentHeldItem.transform.localRotation = Quaternion.identity;
                currentHeldItem.transform.localScale = Vector3.one;

                // Disable unnecessary components (e.g., colliders, rigidbodies)
                Collider collider = currentHeldItem.GetComponent<Collider>();
                if (collider != null) collider.enabled = false;

                Rigidbody rigidbody = currentHeldItem.GetComponent<Rigidbody>();
                if (rigidbody != null) rigidbody.isKinematic = true;
            }
        }

        private void OnItemSelected(object sender, InventoryEventArgs e)
        {
            DisplaySelectedItemInHand();
        }

        private IEnumerator ScrollCooldownRoutine()
        {
            scrollCooldown = true;
            yield return new WaitForSeconds(0.2f);
            scrollCooldown = false;
        }
    }
}