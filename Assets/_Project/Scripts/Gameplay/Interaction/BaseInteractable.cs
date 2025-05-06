using UnityEngine;

namespace Flat.Gameplay.Interaction
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [Header("Interactable Data")]
        [SerializeField, TextArea] private string prompt;
        [SerializeField] private InteractionType interactionType = InteractionType.Instant;
        [SerializeField, Tooltip("Duration in seconds required to hold for Hold interaction type")]
        private float holdDuration = 1.0f;

        public string InteractionPrompt => prompt;
        public InteractionType InteractionType => interactionType;
        public float HoldDuration => holdDuration;

        public abstract void Interact();

        public virtual void OnInteractionUpdate(float holdTimeNormalized) { 

        }
    }
}
