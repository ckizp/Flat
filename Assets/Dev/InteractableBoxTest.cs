using Flat.Gameplay.Interaction;
using UnityEngine;

namespace Test
{
    public class InteractableBoxTest : BaseInteractable
    {
        public override void Interact()
        {
            Debug.Log("Interaction");
        }
    }
}