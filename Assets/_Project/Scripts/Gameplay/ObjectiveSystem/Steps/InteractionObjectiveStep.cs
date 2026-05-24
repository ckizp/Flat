using Flat.Gameplay.Interaction;
using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem.Steps
{
    public class InteractionObjectiveStep : ObjectiveStep
    {
        [SerializeField] private string targetInteractableId;
        [SerializeField] private string targetInteractionType;

        private void OnEnable()
        {
            BaseInteractable.AnyInteraction += OnInteraction;
        }

        private void OnDisable()
        {
            BaseInteractable.AnyInteraction -= OnInteraction;
        }

        private void OnInteraction(object sender, InteractionEventArgs e)
        {
            // Check interactable ID if specified
            bool idMatches = string.IsNullOrEmpty(targetInteractableId) || e.InteractableId == targetInteractableId;

            // Check interaction type if specified
            bool typeMatches = string.IsNullOrEmpty(targetInteractionType) || e.InteractionType == targetInteractionType;

            Debug.Log($"[InteractionStep] Received interaction: ID='{e.InteractableId}', Type='{e.InteractionType}'. Target: ID='{targetInteractableId}', Type='{targetInteractionType}'. Matches: ID={idMatches}, Type={typeMatches}");

            if (idMatches && typeMatches)
            {
                FinishObjectiveStep();
            }
        }
}
}