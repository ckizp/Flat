using UnityEngine;

namespace Flat.Characters
{
    [RequireComponent(typeof(Animator))]
    public class ActorSitController : MonoBehaviour
    {
        private Animator animator;

        [Header("Sitting Settings")]
        public string sittingParameter = "sitting";
        public bool startSitting = false;

        private bool isSitting = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            Sit(startSitting);
        }

        public void Sit(bool sit)
        {
            isSitting = sit;
            animator.SetBool(sittingParameter, isSitting);
        }

        public void ToggleSit()
        {
            Sit(!isSitting);
        }

        public bool IsSitting()
        {
            return isSitting;
        }
    }
}
