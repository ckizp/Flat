using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace Flat.Gameplay.Characters
{
    public class PlayerSound : MonoBehaviour
    {
        [Header("Footsteps")]
        [SerializeField] private List<AudioClip> woodFS;
        [SerializeField] private List<AudioClip> carpetFS;
        private float lastFootstepTime = 0f;
        [SerializeField] private float footstepCooldown = 0.2f;

        [Header("FMOD - Breathing")]
        [SerializeField] private EventReference breathingEvent;
        [SerializeField, Tooltip("Name of the Anxiety parameter in FMOD")]
        private string anxietyParameterName = "Anxiety";

        private EventInstance _breathingInstance;
        private PlayerAnxietyController _anxietyController;

        private enum FSMaterial { 
            WOOD,
            CARPET,
            EMPTY
        }

        private AudioSource footstepSource;

        void Start()
        {
            footstepSource = GetComponent<AudioSource>();
            InitializeBreathing();
        }

        private void OnDestroy()
        {
            StopBreathing();
        }

        void Update()
        {
            UpdateBreathingAnxiety();
        }

        #region FMOD Breathing

        private void InitializeBreathing()
        {
            // Get reference to anxiety controller
            _anxietyController = GetComponent<PlayerAnxietyController>();
            if (_anxietyController == null)
            {
                _anxietyController = GetComponentInParent<PlayerAnxietyController>();
            }

            if (_anxietyController == null)
            {
                Debug.LogWarning("PlayerSound: No PlayerAnxietyController found. Breathing will not respond to anxiety.");
            }

            // Create and start the FMOD breathing event
            if (!breathingEvent.IsNull)
            {
                _breathingInstance = RuntimeManager.CreateInstance(breathingEvent);
                _breathingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                _breathingInstance.start();
            }
            else
            {
                Debug.LogWarning("PlayerSound: Breathing event reference is not set.");
            }
        }

        private void UpdateBreathingAnxiety()
        {
            if (!_breathingInstance.isValid()) return;

            // Update 3D position
            _breathingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            // Update anxiety parameter
            if (_anxietyController != null)
            {
                _breathingInstance.setParameterByName(anxietyParameterName, _anxietyController.CurrentAnxiety);
            }
        }

        /// Manually set the breathing anxiety level (useful if not using PlayerAnxietyController)
        public void SetBreathingAnxiety(float anxiety)
        {
            if (_breathingInstance.isValid())
            {
                _breathingInstance.setParameterByName(anxietyParameterName, Mathf.Clamp(anxiety, 0f, 100f));
            }
        }

        private void StopBreathing()
        {
            if (_breathingInstance.isValid())
            {
                _breathingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _breathingInstance.release();
            }
        }

        /// Pause or resume the breathing sound
        public void SetBreathingPaused(bool paused)
        {
            if (_breathingInstance.isValid())
            {
                _breathingInstance.setPaused(paused);
            }
        }

        #endregion

        #region Footsteps

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

        #endregion
    }
}
