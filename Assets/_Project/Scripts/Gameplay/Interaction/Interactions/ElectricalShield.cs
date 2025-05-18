using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flat.Gameplay.Interaction.Interactions
{
    public class ElectricalShield : BaseInteractable
    {
        [Header("Visual Elements")]
        [SerializeField] private Animator animator;
        [SerializeField] private Animator powerAnimator;
        [SerializeField] private GameObject redPointLight;
        [SerializeField] private GameObject greenPointLight;

        [Header("Breaker Properties")]
        [SerializeField] private List<GameObject> powerObjects = new List<GameObject>();

        public bool isTurning { get; private set; }

        public override void Interact()
        {
            if (isTurning == false)
            {
                StartCoroutine(TurnOn());
            }
            else
            {
                StartCoroutine(TurnOff());
            }
        }

        public IEnumerator TurnOn()
        {
            animator.Play("TurnOn");
            powerAnimator.Play("PowerOn");
            redPointLight.SetActive(false);
            greenPointLight.SetActive(true);
            foreach (GameObject obj in powerObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
            isTurning = true;
            TriggerInteraction("breaker_activate");
            yield return new WaitForSeconds(.5f);
        }

        public IEnumerator TurnOff()
        {
            animator.Play("TurnOff");
            powerAnimator.Play("PowerOff");
            redPointLight.SetActive(true);
            greenPointLight.SetActive(false);
            foreach (GameObject obj in powerObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
            isTurning = false;
            yield return new WaitForSeconds(.5f);
        }
    }
}