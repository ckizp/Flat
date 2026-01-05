using System;
using Flat.Gameplay.Inventory.Items;
using Flat.Gameplay.ObjectiveSystem;
using Flat.Gameplay.Interaction.Interactions;
using Flat.Managers;
using UnityEngine;
using UnityEngine.Video;

namespace Flat.Gameplay.Inventory.Items
{
    public class TVController : MonoBehaviour
    {
        [Header("Video Settings")]
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private VideoClip videoToPlay;
        [SerializeField] private GameObject screen;
        [SerializeField] private float videoBrightness = 50f;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource tvAudio;
        [SerializeField] private AudioClip endingSound;

        [Header("Objective Settings")]
        [SerializeField] private string requiredObjectiveId = "Objective_WatchingYou";
        [SerializeField] private int requiredStepIndex = 0;

        [Header("Door Settings")]
        [SerializeField] private LockedDoor neighborDoor;

        private bool hasPlayed = false;
        private bool isVideoPlaying = false;
        private bool isCorrectStepActive = false;
        private Material screenMaterial;

        public static event Action OnVideoEnded;

        private void Awake()
        {
            if (screen != null)
            {
                Renderer renderer = screen.GetComponent<Renderer>();
                if (renderer != null)
                {
                    screenMaterial = renderer.material;
                }
            }
        }

        private void OnEnable()
        {
            TvRemoteItem.OnRemoteUsed += HandleRemoteUsed;
            GameManager.Instance.ObjectiveEvents.OnObjectiveStateChange += HandleObjectiveStateChange;
            GameManager.Instance.ObjectiveEvents.OnAdvanceObjective += HandleObjectiveAdvance;

            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached += OnVideoFinished;
            }
        }

        private void OnDisable()
        {
            TvRemoteItem.OnRemoteUsed -= HandleRemoteUsed;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ObjectiveEvents.OnObjectiveStateChange -= HandleObjectiveStateChange;
                GameManager.Instance.ObjectiveEvents.OnAdvanceObjective -= HandleObjectiveAdvance;
            }

            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoFinished;
            }
        }

        private void HandleObjectiveStateChange(Objective objective)
        {
            if (objective.info.Id == requiredObjectiveId && objective.state == ObjectiveState.IN_PROGRESS)
            {
                isCorrectStepActive = (requiredStepIndex == 0);
            }
            else if (objective.info.Id == requiredObjectiveId && objective.state == ObjectiveState.FINISHED)
            {
                isCorrectStepActive = false;
            }
        }

        private void HandleObjectiveAdvance(string objectiveId)
        {
            if (objectiveId == requiredObjectiveId)
            {
                isCorrectStepActive = false;
            }
        }

        private void HandleRemoteUsed()
        {
            if (hasPlayed) return;
            if (screen == null || !screen.activeInHierarchy) return;
            if (!isCorrectStepActive) return;

            hasPlayed = true;
            isVideoPlaying = true;

            if (tvAudio != null)
            {
                tvAudio.Stop();
            }

            if (screenMaterial != null)
            {
                screenMaterial.SetFloat("_Brightness", videoBrightness);
            }

            if (videoPlayer != null && videoToPlay != null)
            {
                videoPlayer.clip = videoToPlay;
                videoPlayer.Play();
            }
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            if (!isVideoPlaying) return;
            isVideoPlaying = false;

            if (screen != null)
            {
                screen.SetActive(false);
            }

            if (tvAudio != null && endingSound != null)
            {
                tvAudio.PlayOneShot(endingSound);
            }

            if (neighborDoor != null)
            {
                neighborDoor.Unlock();
            }

            OnVideoEnded?.Invoke();
        }
    }
}