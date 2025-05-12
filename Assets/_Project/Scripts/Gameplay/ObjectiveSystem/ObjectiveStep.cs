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
                GameManager.Instance.ObjectiveEvents.AdvanceObjective(objectiveId);
                Destroy(this.gameObject);
            }
        }
    }
}
