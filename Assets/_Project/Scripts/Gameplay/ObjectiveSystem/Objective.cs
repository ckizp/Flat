using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem
{
    public class Objective
    {
        public ObjectiveInfoSO info;
        public ObjectiveState state;
        private int currentObjectiveStepIndex;

        public Objective(ObjectiveInfoSO objectiveInfo)
        {
            this.info = objectiveInfo;
            this.state = ObjectiveState.INACTIVE;
            this.currentObjectiveStepIndex = 0;
        }

        public void MoveToNextStep()
        {
            currentObjectiveStepIndex++;
        }

        public bool CurrentStepExists()
        {
            return (currentObjectiveStepIndex < info.objectiveStepPrefabs.Length);
        }

        public void InstantiateCurrentObjectiveStep(Transform parentTransform)
        {
            GameObject objectiveStepPrefab = GetCurrentObjectiveStepPrefab();
            if (objectiveStepPrefab != null)
            {
                ObjectiveStep objectiveStep = Object.Instantiate<GameObject>(objectiveStepPrefab, parentTransform)
                    .GetComponent<ObjectiveStep>();
                objectiveStep.InitializeObjectiveStep(info.Id);
            }
        }

        public GameObject GetCurrentObjectiveStepPrefab()
        {
            GameObject objectiveStepPrefab = null;
            if (CurrentStepExists())
            {
                objectiveStepPrefab = info.objectiveStepPrefabs[currentObjectiveStepIndex];
            }
            else
            {
                Debug.LogWarning("Tried to get step prefab, but stepIndex was out of range indicating that "
                    + "there's no current step: ObjectiveId=" + info.Id + ", stepIndex=" + currentObjectiveStepIndex);
            }
            return objectiveStepPrefab;
        }
    }
}
