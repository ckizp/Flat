using Flat.Managers;
using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem
{
    public abstract class ObjectiveStep : MonoBehaviour
    {
        [SerializeField] protected string stepDescription;

        public string StepDescription => stepDescription;

        private bool isFinished = false;
        private string objectiveId;

        public void InitializeObjectiveStep(string objectiveId)
        {
            this.objectiveId = objectiveId;
        }

        protected void FinishObjectiveStep()
        {
            if (!isFinished)
            {
                isFinished = true;
                Debug.Log($"[ObjectiveStep] Finishing step '{stepDescription}' for objective '{objectiveId}'");
                if (GameManager.Instance != null && GameManager.Instance.ObjectiveEvents != null)
                {
                    GameManager.Instance.ObjectiveEvents.AdvanceObjective(objectiveId);
                }
                else
                {
                    Debug.LogError($"[ObjectiveStep] GameManager or ObjectiveEvents is NULL when trying to finish step!");
                }
                Destroy(this.gameObject);
            }
        }
}
}
