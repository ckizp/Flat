using System.Collections;
using Flat.Gameplay.ObjectiveSystem;
using Flat.Managers;
using TMPro;
using UnityEngine;

namespace Flat.UI
{
    public class ObjectiveUIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject objectiveContainer;
        [SerializeField] private TextMeshProUGUI objectiveTitleText;
        [SerializeField] private TextMeshProUGUI objectiveStepText;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private float stepTransitionDuration = 0.25f;

        private CanvasGroup canvasGroup;
        private Objective currentObjective;
        private Coroutine fadeCoroutine;
        private Coroutine stepTransitionCoroutine;

        private void Awake()
        {
            canvasGroup = objectiveContainer.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ObjectiveEvents.OnObjectiveStateChange += HandleObjectiveStateChanged;
                GameManager.Instance.ObjectiveEvents.OnAdvanceObjective += HandleObjectiveAdvance;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ObjectiveEvents.OnObjectiveStateChange -= HandleObjectiveStateChanged;
                GameManager.Instance.ObjectiveEvents.OnAdvanceObjective -= HandleObjectiveAdvance;
            }
        }

        private void HandleObjectiveStateChanged(Objective objective)
        {
            currentObjective = objective;

            switch (objective.state)
            {
                case ObjectiveState.IN_PROGRESS:
                    UpdateObjectiveUI(objective);
                    ShowObjectiveUI();
                    break;

                case ObjectiveState.FINISHED:
                    HideObjectiveUI();
                    break;

                case ObjectiveState.INACTIVE:
                    break;
            }
        }

        private void HandleObjectiveAdvance(string objectiveId)
        {
            if (currentObjective != null && currentObjective.info.Id == objectiveId)
            {
                StartCoroutine(WaitForStepInstantiation());
            }
        }

        private IEnumerator WaitForStepInstantiation()
        {
            yield return null;

            if (currentObjective.CurrentStepExists())
            {
                GameObject stepPrefab = currentObjective.GetCurrentObjectiveStepPrefab();
                if (stepPrefab != null)
                {
                    ObjectiveStep stepComponent = stepPrefab.GetComponent<ObjectiveStep>();
                    if (stepComponent != null)
                    {
                        UpdateStepText(stepComponent.StepDescription);
                    }
                }
            }
        }

        private void UpdateObjectiveUI(Objective objective)
        {
            objectiveTitleText.text = objective.info.displayName;

            if (objective.CurrentStepExists())
            {
                GameObject stepPrefab = objective.GetCurrentObjectiveStepPrefab();
                if (stepPrefab != null)
                {
                    ObjectiveStep stepComponent = stepPrefab.GetComponent<ObjectiveStep>();
                    if (stepComponent != null)
                    {
                        objectiveStepText.text = stepComponent.StepDescription;
                    }
                }
            }
            else
            {
                objectiveStepText.text = "";
            }
        }

        private void UpdateStepText(string newStepText)
        {
            if (stepTransitionCoroutine != null)
            {
                StopCoroutine(stepTransitionCoroutine);
            }

            stepTransitionCoroutine = StartCoroutine(TransitionStepText(newStepText));
        }

        private void ShowObjectiveUI()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeIn());
        }

        private void HideObjectiveUI()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOut());
        }

        private IEnumerator FadeIn()
        {
            objectiveContainer.SetActive(true);
            canvasGroup.alpha = 0f;

            float startTime = Time.time;
            while (Time.time < startTime + fadeInDuration)
            {
                float progress = (Time.time - startTime) / fadeInDuration;
                canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, progress);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            fadeCoroutine = null;
        }

        private IEnumerator FadeOut()
        {
            canvasGroup.alpha = 1f;

            float startTime = Time.time;
            while (Time.time < startTime + fadeOutDuration)
            {
                float progress = (Time.time - startTime) / fadeOutDuration;
                canvasGroup.alpha = Mathf.SmoothStep(1f, 0f, progress);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            objectiveContainer.SetActive(false);
            fadeCoroutine = null;
        }

        private IEnumerator TransitionStepText(string newStepText)
        {
            float startTime = Time.time;
            float startAlpha = objectiveStepText.alpha;

            while (Time.time < startTime + stepTransitionDuration * 0.5f)
            {
                float progress = (Time.time - startTime) / (stepTransitionDuration * 0.5f);
                objectiveStepText.alpha = Mathf.SmoothStep(startAlpha, 0f, progress);
                yield return null;
            }

            objectiveStepText.text = newStepText;

            startTime = Time.time;
            while (Time.time < startTime + stepTransitionDuration * 0.5f)
            {
                float progress = (Time.time - startTime) / (stepTransitionDuration * 0.5f);
                objectiveStepText.alpha = Mathf.SmoothStep(0f, 1f, progress);
                yield return null;
            }

            objectiveStepText.alpha = 1f;
            stepTransitionCoroutine = null;
        }
    }
}