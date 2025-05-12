using UnityEngine;

namespace Flat.Gameplay.ObjectiveSystem
{
    [CreateAssetMenu(fileName = "ObjectiveInfoSO", menuName = "Flat/Objective System/ObjectiveInfoSO")]
    public class ObjectiveInfoSO : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }

        [Header("General")]
        public string displayName;

        [Header("Steps")]
        public GameObject[] objectiveStepPrefabs;

        // Ensure the Id is always the name of the Scriptable Object asset
        private void OnValidate()
        {
#if UNITY_EDITOR
            Id = this.name;
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
