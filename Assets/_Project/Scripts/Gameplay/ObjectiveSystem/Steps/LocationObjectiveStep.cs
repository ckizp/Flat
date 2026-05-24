using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem.Steps
{
    /// Objective step that completes when the player enters a trigger zone
    [RequireComponent(typeof(Collider))]
    public class LocationObjectiveStep : ObjectiveStep
    {
        [Header("Transform Settings")]
        [SerializeField, Tooltip("World position where the trigger will be placed")]
        private Vector3 worldPosition;

        [SerializeField, Tooltip("Scale to apply to the trigger")]
        private Vector3 scale = Vector3.one;

        private Collider triggerZone;

        private void Awake()
        {
            triggerZone = GetComponent<Collider>();
            triggerZone.isTrigger = true;

            // Detach from parent temporarily to apply world position
            Transform originalParent = transform.parent;
            transform.SetParent(null);

            // Apply world position and scale
            transform.position = worldPosition;
            transform.localScale = scale;

            // Reattach to original parent if it existed
            if (originalParent != null)
            {
                transform.SetParent(originalParent, true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player")) || other.transform.root.CompareTag("Player"))
            {
                FinishObjectiveStep();
            }
        }
}
}