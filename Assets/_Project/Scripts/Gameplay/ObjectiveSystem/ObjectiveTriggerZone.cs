using Flat.Managers;
using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem
{
    [RequireComponent(typeof(Collider))]
    public class ObjectiveTriggerZone : MonoBehaviour
    {
        [Header("Trigger Settings")]
        [SerializeField, Tooltip("ID of the objective to start")]
        private string objectiveToStart;

        [SerializeField, Tooltip("ID of the objective that must be completed before enabling this trigger (leave empty if none)")]
        private string prerequisiteObjectiveId;

        [SerializeField, Tooltip("Destroy the trigger after activation")]
        private bool destroyOnTrigger = true;

        private Collider triggerCollider;
        private bool isActivated = false;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;

            // Disable if a prerequisite exists
            if (!string.IsNullOrEmpty(prerequisiteObjectiveId))
            {
                triggerCollider.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ObjectiveEvents.OnObjectiveStateChange += HandleObjectiveStateChanged;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ObjectiveEvents.OnObjectiveStateChange -= HandleObjectiveStateChanged;
            }
        }

        private void HandleObjectiveStateChanged(Objective objective)
        {
            // Enable trigger when the prerequisite objective is completed
            if (!string.IsNullOrEmpty(prerequisiteObjectiveId) &&
                objective.info.Id == prerequisiteObjectiveId &&
                objective.state == ObjectiveState.FINISHED)
            {
                triggerCollider.enabled = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isActivated) return;
            if (!other.CompareTag("Player")) return;

            isActivated = true;
            GameManager.Instance.ObjectiveEvents.StartObjective(objectiveToStart);

            if (destroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}