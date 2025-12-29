using System.Collections;
using UnityEngine;

namespace Flat.Gameplay.Characters
{
    /// Trigger for modifying player anxiety.
    /// Can be used as a zone (resets on exit) or a transition (persistent).
    [RequireComponent(typeof(Collider))]
    public class AnxietyTrigger : MonoBehaviour
    {
        [Header("Anxiety Settings")]
        [SerializeField, Range(0f, 100f), Tooltip("Base anxiety level when triggered")]
        private float anxietyLevel = 50f;

        [SerializeField, Tooltip("Reset anxiety to 0 when player exits (Zone mode)")]
        private bool resetOnExit = true;

        [Header("Spike (Optional)")]
        [SerializeField, Tooltip("Also add an anxiety spike when entering")]
        private bool addSpike = false;

        [SerializeField, Range(0f, 100f), Tooltip("Spike amount to add")]
        private float spikeAmount = 30f;

        [Header("Timing")]
        [SerializeField, Tooltip("Delay in seconds before the effect triggers")]
        private float delay = 0f;

        [Header("Trigger Behavior")]
        [SerializeField, Tooltip("If true, only triggers once")]
        private bool triggerOnce = false;

        private bool _hasTriggered;
        private Coroutine _delayCoroutine;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (triggerOnce && _hasTriggered) return;

            if (delay > 0f)
            {
                _delayCoroutine = StartCoroutine(ApplyAfterDelay());
            }
            else
            {
                ApplyAnxiety();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (resetOnExit && !addSpike && _delayCoroutine == null)
            {
                PlayerAnxietyController.Instance?.SetBaseAnxiety(0f);
            }
        }

        private IEnumerator ApplyAfterDelay()
        {
            yield return new WaitForSeconds(delay);

            ApplyAnxiety();

            _delayCoroutine = null;
        }

        private void ApplyAnxiety()
        {
            var controller = PlayerAnxietyController.Instance;
            if (controller == null) return;

            controller.SetBaseAnxiety(anxietyLevel);

            if (addSpike)
            {
                controller.AddAnxietySpike(spikeAmount);
            }

            if (triggerOnce)
            {
                _hasTriggered = true;
            }
        }

        public void ResetTrigger() => _hasTriggered = false;
    }
}
