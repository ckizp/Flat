using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flat.Environment
{
    public class LightFlickerController : MonoBehaviour
    {
        [SerializeField] private List<Light> lights;
        [SerializeField] private float minInterval = 3f;
        [SerializeField] private float maxInterval = 8f;
        [SerializeField] private float flickerDuration = 1.5f;

        private void Start()
        {
            StartCoroutine(FlickerRoutine());
        }

        IEnumerator FlickerRoutine()
        {
            while (true)
            {
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                int lightIndex = Random.Range(0, lights.Count);
                Light chosenLight = lights[lightIndex];

                StartCoroutine(FlickerLight(chosenLight));
            }
        }

        IEnumerator FlickerLight(Light light)
        {
            float elapsed = 0f;
            float flickerSpeed = Random.Range(0.05f, 0.2f);

            while (elapsed < flickerDuration)
            {
                light.enabled = !light.enabled;
                yield return new WaitForSeconds(flickerSpeed);

                elapsed += flickerSpeed;
                flickerSpeed = Random.Range(0.05f, 0.15f);
            }

            light.enabled = true;
        }
    }
}