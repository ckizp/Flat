using System.Collections;
using System.Collections.Generic;
using Flat.Gameplay.Inventory;
using Flat.Gameplay.Inventory.Implementations;
using Flat.Managers;
using UnityEngine;

namespace Flat.Gameplay
{
    public class GameStarter : MonoBehaviour
    {
        [Header("Power Outage Settings")]
        [SerializeField] private float delayBeforePowerOutage = 10f;
        [SerializeField] private AudioSource powerOutageSound;
        [SerializeField] private List<GameObject> powerObjects = new List<GameObject>();

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
            if (powerOutageSound != null)
            {
                powerOutageSound.Play();

                GameObject player = GameObject.FindGameObjectWithTag("Player");

                PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
                
                if (flashlightItemPrefab != null)
                {
                    playerInventory.AddItem(flashlightItemPrefab);
                }
            }

            foreach (GameObject obj in powerObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}