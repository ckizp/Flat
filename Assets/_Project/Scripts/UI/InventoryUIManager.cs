using System.Collections.Generic;
using Flat.Gameplay.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Flat.UI
{
    public class InventoryUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Color selectedColor = new Color(0.58f, 0.58f, 0.58f, 0.82f);
        [SerializeField] private Color normalColor = new Color(0f, 0f, 0f, 0.55f);

        private PlayerInventory playerInventory;

        private void Start()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }

            if (player != null)
            {
                playerInventory = player.GetComponentInChildren<PlayerInventory>();
                if (playerInventory == null)
                {
                    Debug.LogError("Player doesn't have an Inventory component in children!");
                    return;
                }

                playerInventory.ItemAdded += OnItemAdded;
                playerInventory.ItemRemoved += OnItemRemoved;
                playerInventory.ItemSelected += OnItemSelected;
            }

            InitializeSlots();
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
            {
                playerInventory.ItemAdded -= OnItemAdded;
                playerInventory.ItemRemoved -= OnItemRemoved;
                playerInventory.ItemSelected -= OnItemSelected;
            }
        }

        private void InitializeSlots()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform slotTransform = transform.GetChild(i);

                Image itemImage = slotTransform.GetChild(0).GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.enabled = false;
                }

                Image slotImage = slotTransform.GetComponent<Image>();
                if (slotImage != null)
                {
                    slotImage.color = normalColor;
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Item item = playerInventory.GetItemAt(i);

                if (item != null)
                {
                    Transform slotTransform = transform.GetChild(i);
                    Image itemImage = slotTransform.GetChild(0).GetComponent<Image>();

                    itemImage.enabled = true;
                    itemImage.sprite = item.icon;
                }
            }

            int selectedIndex = playerInventory.GetSelectedIndex();
            if (selectedIndex >= 0 && selectedIndex < transform.childCount)
            {
                transform.GetChild(selectedIndex).GetComponent<Image>().color = selectedColor;
            }
        }

        private void OnItemAdded(object sender, InventoryEventArgs e)
        {
            UpdateInventoryUI();
        }

        private void OnItemRemoved(object sender, InventoryEventArgs e)
        {
            UpdateInventoryUI();
        }

        private void OnItemSelected(object sender, InventoryEventArgs e)
        {
            UpdateSelectionIndicator();
        }

        private void UpdateInventoryUI()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform slotTransform = transform.GetChild(i);

                Item item = playerInventory.GetItemAt(i);
                Image itemImage = slotTransform.GetChild(0).GetComponent<Image>();

                if (item != null)
                {
                    itemImage.enabled = true;
                    itemImage.sprite = item.icon;
                }
                else
                {
                    itemImage.enabled = false;
                }
            }

            UpdateSelectionIndicator();
        }

        private void UpdateSelectionIndicator()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Image>().color = normalColor;
            }

            int selectedIndex = playerInventory.GetSelectedIndex();
            if (selectedIndex >= 0 && selectedIndex < transform.childCount)
            {
                transform.GetChild(selectedIndex).GetComponent<Image>().color = selectedColor;
            }
        }
    }
}