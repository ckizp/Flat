using System;
using UnityEngine;

namespace Flat.Gameplay.Interaction
{
    public class InteractionEventArgs : EventArgs
    {
        public string InteractableId { get; private set; }
        public string InteractionType { get; private set; }

        public InteractionEventArgs(string interactableId, string interactionType)
        {
            InteractableId = interactableId;
            InteractionType = interactionType;
        }
    }

    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [Header("Interactable Data")]
        [SerializeField] private string interactableId;
        [SerializeField, TextArea] private string prompt;
        [SerializeField] private InteractionType interactionType = InteractionType.Instant;
        [SerializeField, Tooltip("Duration in seconds required to hold for Hold interaction type")]
        private float holdDuration = 1.0f;

        public static event EventHandler<InteractionEventArgs> AnyInteraction;

        public string InteractionPrompt => prompt;
        public InteractionType InteractionType => interactionType;
        public float HoldDuration => holdDuration;

        public abstract void Interact();

        public virtual void OnInteractionUpdate(float holdTimeNormalized) { 

        }

        protected void TriggerInteraction(string actionType)
        {
            if (!string.IsNullOrEmpty(interactableId))
            {
                var args = new InteractionEventArgs(interactableId, actionType);
                AnyInteraction?.Invoke(this, args);
            }
        }
    }
}
