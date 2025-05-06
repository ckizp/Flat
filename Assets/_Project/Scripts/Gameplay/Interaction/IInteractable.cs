namespace Flat.Gameplay.Interaction
{
    public enum InteractionType
    {
        Instant,    // Single press interaction
        Hold        // Hold button to interact
    }

    public interface IInteractable
    {
        string InteractionPrompt { get; }
        InteractionType InteractionType { get; }
        float HoldDuration { get; }
        void Interact();
        void OnInteractionUpdate(float holdTimeNormalized);
    }
}