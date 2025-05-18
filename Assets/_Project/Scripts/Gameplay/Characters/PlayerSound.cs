using System.Collections.Generic;
using UnityEngine;

namespace Flat.Gameplay.Characters
{
    public class PlayerSound : MonoBehaviour
    {
        [Header("Footsteps")]
        [SerializeField] private List<AudioClip> woodFS;
        [SerializeField] private List<AudioClip> carpetFS;
        private float lastFootstepTime = 0f;
        [SerializeField] private float footstepCooldown = 0.2f;

        private enum FSMaterial { 
            WOOD,
            CARPET,
            EMPTY
        }

        private AudioSource footstepSource;

        void Start()
        {
            footstepSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private FSMaterial SurfaceSelect()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);
            Material surfaceMaterial;

            if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                Renderer surfaceRenderer = hit.collider.GetComponentInChildren<Renderer>();
                if (surfaceRenderer)
                {
                    surfaceMaterial = surfaceRenderer.sharedMaterial;

                    if (surfaceMaterial.name.Contains("Wood"))
                    {
                        return FSMaterial.WOOD;
                    }
                    else if (surfaceMaterial.name.Contains("Carpet"))
                    {
                        return FSMaterial.CARPET;
                    }
                    else
                    {
                        return FSMaterial.EMPTY;
                    }
                }
            }

            return FSMaterial.EMPTY;
        }

        private void PlayFootstep()
        {
            if (Time.time - lastFootstepTime < footstepCooldown) return;
            lastFootstepTime = Time.time;


            AudioClip clip = null;

            FSMaterial surface = SurfaceSelect();

            switch (surface)
            {
                case FSMaterial.WOOD:
                    clip = woodFS[Random.Range(0, woodFS.Count)];
                    break;
                case FSMaterial.CARPET:
                    clip = carpetFS[Random.Range(0, carpetFS.Count)];
                    break;
                default:
                    break;
            }

            if (clip != null)
            {
                footstepSource.clip = clip;
                footstepSource.volume = Random.Range(0.03f, 0.06f);
                footstepSource.pitch = Random.Range(0.8f, 1.2f);
                footstepSource.Play();
            }
        }
    }
}
