using Flat.Gameplay.ObjectiveSystem;
using UnityEngine;

namespace Flat.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private ObjectiveEvents _objectiveEvents;
        public ObjectiveEvents ObjectiveEvents => _objectiveEvents;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Found more than one Game Manager in the scene.");
                Destroy(gameObject);
            }
            Instance = this;

            DontDestroyOnLoad(gameObject);

            // Initialize all events
            _objectiveEvents = new ObjectiveEvents();
        }
    }
}
