using Flat.Gameplay.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Flat.UI
{
    public class InteractionUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Interactor interactor;
        [SerializeField] private Slider holdSlider;
        [SerializeField] private RectOffset normalPadding;
        [SerializeField] private RectOffset holdModePadding;

        private VerticalLayoutGroup verticalLayoutGroup;

        void Start()
        {
            verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();

            if (interactor == null)
            {
                Debug.LogError("Interactor reference not set in InteractionUIManager. Please assign it in the inspector.");
                return;
            }

            if (interactor != null)
            {
                interactor.OnFocusEnter += HandleFocusEnter;
                interactor.OnFocusExit += HandleFocusExit;
                interactor.OnHoldProgress += UpdateHoldProgress;
            }

            if (holdSlider != null)
            {
                holdSlider.gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (interactor != null)
            {
                interactor.OnFocusEnter -= HandleFocusEnter;
                interactor.OnFocusExit -= HandleFocusExit;
                interactor.OnHoldProgress -= UpdateHoldProgress;
            }
        }

        private void HandleFocusEnter(IInteractable interactable)
        {
            if (promptText != null)
            {
                promptText.text = interactable.InteractionPrompt;
                gameObject.SetActive(true);

                if (holdSlider != null)
                {
                    holdSlider.gameObject.SetActive(interactable.InteractionType == InteractionType.Hold);
                    holdSlider.value = 0;
                    verticalLayoutGroup.padding = interactable.InteractionType == InteractionType.Hold ? holdModePadding : normalPadding;
                }
            }
        }

        private void HandleFocusExit()
        {
            gameObject.SetActive(false);
        }

        private void UpdateHoldProgress(float progress)
        {
            if (holdSlider != null && holdSlider.gameObject.activeSelf)
            {
                holdSlider.value = progress;
            }
        }
    }
}
