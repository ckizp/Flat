using System.Collections;
using Flat.Gameplay.Interaction;
using UnityEngine;

namespace Flat.Gameplay.Interaction.Implementations
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
            yield return new WaitForSeconds(.5f);
        }

        private IEnumerator Close()
        {
            animator.Play(closingAnimationName);
            IsOpen = false;
            yield return new WaitForSeconds(.5f);
        }
    }
}