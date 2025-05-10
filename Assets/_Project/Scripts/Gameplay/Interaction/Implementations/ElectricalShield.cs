using System.Collections;
using UnityEngine;

namespace Flat.Gameplay.Interaction.Implementations
{
    public class ElectricalShield : BaseInteractable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Animator powerAnimator;
        [SerializeField] private GameObject redPointLight;
        [SerializeField] private GameObject greenPointLight;

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

        private IEnumerator TurnOn()
        {
            animator.Play("TurnOn");
            powerAnimator.Play("PowerOn");
            redPointLight.SetActive(false);
            greenPointLight.SetActive(true);
            isTurning = true;
            yield return new WaitForSeconds(.5f);
        }

        private IEnumerator TurnOff()
        {
            animator.Play("TurnOff");
            powerAnimator.Play("PowerOff");
            redPointLight.SetActive(true);
            greenPointLight.SetActive(false);
            isTurning = false;
            yield return new WaitForSeconds(.5f);
        }
    }
}