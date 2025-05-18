using System.Collections;
using System.Collections.Generic;
using Flat.Gameplay.Interaction.Interactions;
using Flat.Gameplay.Inventory;
using Flat.Gameplay.Inventory.Items;
using Flat.Managers;
using UnityEngine;

namespace Flat.Gameplay
{
    public class GameStarter : MonoBehaviour
    {
        [Header("Power Outage Settings")]
        [SerializeField] private float delayBeforePowerOutage = 10f;
        [SerializeField] private AudioSource powerOutageSound;
        [SerializeField] private ElectricalShield electricalShield;

        [Header("References")]
        [SerializeField] private FlashlightItem flashlightItemPrefab;

        private void Start()
        {
            StartCoroutine(IntroSequence());
        }

        private IEnumerator IntroSequence()
        {
            yield return new WaitForSeconds(delayBeforePowerOutage);
            yield return StartCoroutine(TriggerPowerOutage());
            GameManager.Instance.ObjectiveEvents.StartObjective("Objective_RestoreLights");
        }

        private IEnumerator TriggerPowerOutage()
        {
            // Play the power outage sound
            if (powerOutageSound != null)
            {
                powerOutageSound.Play();
            }

            // Find the player and get their inventory
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();

                // Add the flashlight item to the inventory
                if (playerInventory != null && flashlightItemPrefab != null)
                {
                    playerInventory.AddItem(flashlightItemPrefab);
                }
            }

            // Turn off the electrical shield
            if (electricalShield != null)
            {
                yield return StartCoroutine(electricalShield.TurnOff());
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}