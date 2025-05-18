using Flat.Gameplay.Interaction;
using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem.Steps
{
    public class InteractionObjectiveStep : ObjectiveStep
    {
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
            if (e.InteractionType == targetInteractionType)
            {
                FinishObjectiveStep();
            }
        }
    }
}
