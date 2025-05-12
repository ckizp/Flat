using System;

namespace Flat.Gameplay.ObjectiveSystem
{
    public class ObjectiveEvents
    {
        public event Action<string> OnStartObjective;

        public void StartObjective(string id) => OnStartObjective?.Invoke(id);

        public event Action<string> OnAdvanceObjective;

        public void AdvanceObjective(string id) => OnAdvanceObjective?.Invoke(id);

        public event Action<string> OnFinishObjective;

        public void FinishObjective(string id) => OnFinishObjective?.Invoke(id);

        public event Action<Objective> OnObjectiveStateChange;

        public void UpdateObjectiveState(Objective objective) => OnObjectiveStateChange?.Invoke(objective);
    }
}