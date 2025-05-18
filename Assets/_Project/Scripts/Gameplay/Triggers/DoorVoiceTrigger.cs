using System.Collections;
using UnityEngine;

namespace Flat.Gameplay.Triggers
{
    public class DoorVoiceTrigger : MonoBehaviour
    {
        [SerializeField] private AudioSource doorAs;
        [SerializeField] private AudioSource voiceAs;
        [SerializeField] private float delayBetweenSounds = 1f;

        private bool hasPlayed;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (!hasPlayed)
            {
                hasPlayed = true;
                StartCoroutine(TriggerVoiceSequence());
            }
        }

        private IEnumerator TriggerVoiceSequence()
        {
            doorAs.Play();

            yield return new WaitForSeconds(delayBetweenSounds);

            voiceAs.Play();
        }
    }
}