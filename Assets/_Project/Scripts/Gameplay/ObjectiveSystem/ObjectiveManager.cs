using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flat.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Flat.Gameplay.ObjectiveSystem
{
    public enum GameAct
    {
        Act1,
        Act2,
        Act3
    }

    public class ObjectiveManager : MonoBehaviour
    {
        [SerializeField] private GameAct act;

        private Dictionary<string, Objective> objectiveMap;
        private bool isInitialized = false;

        public bool IsInitialized => isInitialized;

        private void Awake()
        {
            objectiveMap = new Dictionary<string, Objective>();
            StartCoroutine(InitializeObjectiveSystem());
        }

        private void OnDestroy()
        {
            // Se désabonner proprement des événements
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ObjectiveEvents.OnStartObjective -= StartObjective;
                GameManager.Instance.ObjectiveEvents.OnAdvanceObjective -= AdvanceObjective;
                GameManager.Instance.ObjectiveEvents.OnFinishObjective -= FinishObjective;
            }
        }

        private IEnumerator InitializeObjectiveSystem()
        {
            yield return StartCoroutine(LoadObjectives());

            // Wait for GameManager to be ready
            while (GameManager.Instance == null)
            {
                yield return null;
            }

            GameManager.Instance.ObjectiveEvents.OnStartObjective += StartObjective;
            GameManager.Instance.ObjectiveEvents.OnAdvanceObjective += AdvanceObjective;
            GameManager.Instance.ObjectiveEvents.OnFinishObjective += FinishObjective;

            foreach (Objective objective in objectiveMap.Values)
            {
                GameManager.Instance.ObjectiveEvents.UpdateObjectiveState(objective);
            }

            isInitialized = true;
        }

        private void ChangeObjectiveState(string id, ObjectiveState state)
        {
            Objective objective = GetObjectiveById(id);
            if (objective == null) return;
            
            objective.state = state;
            GameManager.Instance.ObjectiveEvents.UpdateObjectiveState(objective);
        }

        private void StartObjective(string id)
        {
            if (!isInitialized) return;

            Objective objective = GetObjectiveById(id);
            if (objective == null) return;

            objective.InstantiateCurrentObjectiveStep(this.transform);
            ChangeObjectiveState(objective.info.Id, ObjectiveState.IN_PROGRESS);
        }

        private void AdvanceObjective(string id)
        {
            Debug.Log($"[ObjectiveManager] Advancing objective: {id}");
            Objective objective = GetObjectiveById(id);
            if (objective == null)
            {
                Debug.LogWarning($"[ObjectiveManager] Objective '{id}' not found!");
                return;
            }

            // Move on to the next step
            objective.MoveToNextStep();

            // If there are more steps, instantiate the next one
            if (objective.CurrentStepExists())
            {
                Debug.Log($"[ObjectiveManager] Instantiating next step for {id}");
                objective.InstantiateCurrentObjectiveStep(this.transform);
                GameManager.Instance.ObjectiveEvents.UpdateObjectiveState(objective);
            }
            else
            {
                Debug.Log($"[ObjectiveManager] No more steps for {id}. Finishing objective.");
                FinishObjective(objective.info.Id);
            }
        }

        private void FinishObjective(string id)
        {
            Objective objective = GetObjectiveById(id);
            if (objective == null) return;

            ChangeObjectiveState(objective.info.Id, ObjectiveState.FINISHED);
        }

        private IEnumerator LoadObjectives()
        {
            string label = act.ToString();

            AsyncOperationHandle<IList<ObjectiveInfoSO>> handle =
                Addressables.LoadAssetsAsync<ObjectiveInfoSO>(label, null);

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var objectiveInfo in handle.Result)
                {
                    if (!objectiveMap.ContainsKey(objectiveInfo.Id))
                    {
                        objectiveMap.Add(objectiveInfo.Id, new Objective(objectiveInfo));
                        Debug.Log($"Loaded objective: {objectiveInfo.displayName} (ID: {objectiveInfo.Id})");
                    }
                    else
                    {
                        Debug.LogWarning($"[ObjectiveManager] Duplicate objective ID found: {objectiveInfo.Id}. Skipping.");
                    }
                }
            }
else
            {
                Debug.LogError($"Failed to load objectives for act: {label}");
            }
        }

        private Objective GetObjectiveById(string id)
        {
            if (objectiveMap != null && objectiveMap.TryGetValue(id, out Objective objective))
            {
                return objective;
            }
            return null;
        }
    }
}