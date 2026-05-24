using Flat.Gameplay.Characters;
using Flat.Gameplay.Interaction;
using UnityEngine;

namespace Flat.Gameplay.Triggers
{
    public class ShadowController : MonoBehaviour
    {
        [SerializeField] private Animator shadowAnimator;
        [SerializeField] private GameObject shadow;
        [SerializeField] private AudioSource triggerAudio;
        [SerializeField] private float shadowRunSpeed = 10f;
        [SerializeField] private string targetInteractionType;

        [Header("Anxiety Settings")]
        [SerializeField] private float anxietySpike = 100f;

        private static readonly int velHash = Animator.StringToHash("Velocity");
        private bool isTriggered;
        private Vector3 shadowStartPosition;

        private void Start()
        {
            shadowStartPosition = transform.position;
        }

        void Update()
        {
            if (isTriggered)
            {
                shadow.transform.position += shadow.transform.forward * shadowRunSpeed * Time.deltaTime;

                if (Vector3.Distance(shadowStartPosition, shadow.transform.position) > 250)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnEnable()
        {
            BaseInteractable.AnyInteraction += OnInteraction;
        }

        private void OnDisable()
        {
            BaseInteractable.AnyInteraction -= OnInteraction;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isTriggered) return;

            // Check root or parent for Player tag to support VR rigs where child colliders hit the trigger
            if (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player")) || other.transform.root.CompareTag("Player"))
            {
                Debug.Log($"[ShadowController] Triggered by {other.gameObject.name} (Root: {other.transform.root.name})");
                isTriggered = true;

                if (PlayerAnxietyController.Instance != null)
                {
                    PlayerAnxietyController.Instance.AddAnxietySpike(anxietySpike);
                }

                if (triggerAudio != null)
                {
                    triggerAudio.Play();
                }

                if (shadowAnimator != null)
                {
                    shadowAnimator.SetFloat(velHash, 10f);
                }
                
                var col = GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }

        private void OnInteraction(object sender, InteractionEventArgs e)
        {
            if (string.IsNullOrEmpty(targetInteractionType)) return;
            
            if (e.InteractionType == targetInteractionType)
            {
                Debug.Log($"[ShadowController] Interaction '{e.InteractionType}' received. Enabling shadow.");
                var col = GetComponent<Collider>();
                if (col != null) col.enabled = true;
                
                if (shadow != null)
                {
                    shadow.SetActive(true);
                    shadowStartPosition = shadow.transform.position; // Reset start position to current
                }
            }
        }
}
}