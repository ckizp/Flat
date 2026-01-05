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

            if (other.CompareTag("Player"))
            {
                isTriggered = true;

                if (PlayerAnxietyController.Instance != null)
                {
                    PlayerAnxietyController.Instance.AddAnxietySpike(anxietySpike);
                }

                if (triggerAudio != null)
                {
                    triggerAudio.Play();
                }

                shadowAnimator.SetFloat(velHash, 10f);
                GetComponent<Collider>().enabled = false;
            }
        }

        private void OnInteraction(object sender, InteractionEventArgs e)
        {
            if (string.IsNullOrEmpty(targetInteractionType)) return;
            
            if (e.InteractionType == targetInteractionType)
            {
                GetComponent<Collider>().enabled = true;
                shadow.SetActive(true);
            }
        }
    }
}