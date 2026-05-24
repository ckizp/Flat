using System.Collections;
using UnityEngine;

namespace Flat.Gameplay.Interaction.Interactions
{
    public class SimpleOpenableObject : BaseInteractable
    {
        [Header("Door Properties")]
        [SerializeField] private Animator animator;
        [SerializeField] private string openingAnimationName;
        [SerializeField] private string closingAnimationName;

        public bool IsOpen { get; private set; }

        public override void Interact()
        {
            if (IsOpen == false)
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
            animator.Play(openingAnimationName);
            IsOpen = true;
            TriggerInteraction("open");
            yield return new WaitForSeconds(.5f);
        }

        private IEnumerator Close()
        {
            animator.Play(closingAnimationName);
            IsOpen = false;
            TriggerInteraction("close");
            yield return new WaitForSeconds(.5f);
        }
}
}