
using Flat.Gameplay.Interaction;
using Flat.Gameplay.ObjectiveSystem;
using UnityEngine;
public class ShadowSystem : MonoBehaviour
{
    [SerializeField] private GameObject shadow;
    [SerializeField] private float shadowRunSpeed = 10f;
    [SerializeField] private AudioSource triggerAudio;
    [SerializeField] private string targetInteractionType;
    private BoxCollider boxCollider;
    private bool isTriggered = false;
    private Vector3 runDirection;
    public GameObject shadowTarget;

    private Animator shadowAnimator;
    private static readonly int xVelHash = Animator.StringToHash("X_Velocity");
    private static readonly int yVelHash = Animator.StringToHash("Y_Velocity");

    private float shadowMaxDistance = 800f;
    private Vector3 shadowStartPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            runDirection = shadow.transform.forward;
            shadowStartPosition = shadow.transform.position;
            shadowAnimator = shadow.GetComponent<Animator>();

            if (triggerAudio != null && !triggerAudio.isPlaying)
            {
                triggerAudio.Play();
            }
            else
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            GetComponent<Collider>().enabled = false;
        }
    }

    private void Update()
    {
        if (isTriggered && shadow != null)
        {
            shadow.transform.position += runDirection * shadowRunSpeed * Time.deltaTime;
            if (shadowAnimator != null)
            {
                float velocity = shadowRunSpeed;
                shadowAnimator.SetFloat(xVelHash, 0f);
                shadowAnimator.SetFloat(yVelHash, velocity);
            }

            if (Vector3.Distance(shadowStartPosition, shadow.transform.position) >= shadowMaxDistance)
            {
                isTriggered = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && shadow != null)
        {
            Destroy(shadow);
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

    private void OnInteraction(object sender, InteractionEventArgs e)
    {
        if (e.InteractionType == targetInteractionType)
        {
            boxCollider.enabled = true;
            if (shadowTarget != null)
                shadowTarget.SetActive(true);
        }
    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
}
