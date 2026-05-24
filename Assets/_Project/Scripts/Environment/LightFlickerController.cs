using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flat.Environment
{
    public class LightFlickerController : MonoBehaviour
    {
        [SerializeField] private List<Light> lights;
        [SerializeField] private float minInterval = 1f;
        [SerializeField] private float maxInterval = 5f;
        [SerializeField] private float flickerDuration = 0.5f;
        [SerializeField] private AudioSource flickerAudioSource;
        [SerializeField] private AudioClip flickerSound;

        private void Start()
        {
            if (lights == null || lights.Count == 0)
            {
                lights = new List<Light>(Object.FindObjectsByType<Light>(FindObjectsSortMode.None));
                // Filter only lamps if needed, but for now we'll take what we find or rely on manual assignment
            }
            StartCoroutine(FlickerRoutine());
        }

        IEnumerator FlickerRoutine()
        {
            while (true)
            {
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                if (lights.Count > 0)
                {
                    int lightIndex = Random.Range(0, lights.Count);
                    Light chosenLight = lights[lightIndex];
                    if (chosenLight != null)
                        StartCoroutine(FlickerLight(chosenLight));
                }
            }
        }

        IEnumerator FlickerLight(Light light)
        {
            float elapsed = 0f;
            float baseIntensity = light.intensity;

            if (flickerAudioSource != null && flickerSound != null)
                flickerAudioSource.PlayOneShot(flickerSound);

            while (elapsed < flickerDuration)
            {
                light.enabled = !light.enabled;
                float flickerSpeed = Random.Range(0.02f, 0.08f);
                yield return new WaitForSeconds(flickerSpeed);

                elapsed += flickerSpeed;
            }

            light.enabled = true;
            light.intensity = baseIntensity;
        }
}
}