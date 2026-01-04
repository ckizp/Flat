using UnityEngine;

namespace Flat.Environment
{
    [RequireComponent(typeof(Collider))]
    public class SoundController : MonoBehaviour
    {
        [Header("Audio Settings")]
        public AudioSource[] particleSounds;
        [Range(0f, 1f)]
        public float insideVolume = 1f;
        [Range(0f, 1f)]
        public float outsideVolume = 0.1f;
        public float fadeSpeed = 2f;

        private bool playerInside = false;

        void Start()
        {
            Collider col = GetComponent<Collider>();
            if (!col.isTrigger)
            {
                Debug.LogWarning("Collider must be a trigger!", this);
                col.isTrigger = true;
            }
        }

        void Update()
        {
            float targetVolume = playerInside ? insideVolume : outsideVolume;

            // Smooth volume fade for each audio source
            foreach (AudioSource audio in particleSounds)
            {
                if (audio == null) continue;
                audio.volume = Mathf.Lerp(audio.volume, targetVolume, Time.deltaTime * fadeSpeed);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                playerInside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                playerInside = false;
        }
    }
}