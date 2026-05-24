using System;
using System.Collections;
using System.Collections.Generic;
using Flat.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace Flat.Gameplay.Interaction
{
    [RequireComponent(typeof(InputManager))]
    public class Interactor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private Transform source;
        [SerializeField, Tooltip("The detection distance when looking straight ahead")]
        private float minDistance = 0.75f;
        [SerializeField, Tooltip("The detection distance when looking up or down")]
        private float maxDistance = 2f;
        [SerializeField] private LayerMask interactableLayers;
        [SerializeField, Tooltip("The interval (in seconds) between detection raycasts")]
        float detectInterval = 0.1f;

        // Events
        public event Action<IInteractable> OnFocusEnter;
        public event Action OnFocusExit;
        public event Action<float> OnHoldProgress;

        private InputManager inputManager;
        private IInteractable currentFocus;
        private Coroutine detectCoroutine;
        private RaycastHit hit;

        private float interactionHoldTime;
        private bool isHolding;
        private bool hasInteracted;

        private void Awake()
        {
            inputManager = GetComponent<InputManager>();

            if (source == null)
            {
                Debug.LogWarning("Interaction source not assigned. Please assign it in the inspector.");
            }
        }

        private void OnEnable()
        {
            if (detectCoroutine == null)
                detectCoroutine = StartCoroutine(DetectRoutine());
        }

        private void OnDisable()
        {
            if (detectCoroutine != null)
            {
                StopCoroutine(detectCoroutine);
                detectCoroutine = null;
            }

            if (currentFocus != null)
            {
                currentFocus = null;
                OnFocusExit?.Invoke();
            }
        }

        private void Update()
        {
            HandleInteraction();
        }

        private IEnumerator DetectRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(detectInterval);

            while (true)
            {
                Detect();
                yield return wait;
            }
        }

        private void Detect()
        {
            float verticalLook = Mathf.Abs(source.forward.y);
            float adjustedDistance = Mathf.Lerp(minDistance, maxDistance, verticalLook);

            Debug.DrawRay(source.position, source.forward * adjustedDistance, Color.red);
            
            if (Physics.Raycast(source.position, source.forward,
                out hit, adjustedDistance, interactableLayers))
            {
                var interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    if (interactable != currentFocus)
                    {
                        currentFocus = interactable;
                        OnFocusEnter?.Invoke(interactable);
                    }
                    return;
                }
            }

            if (currentFocus != null)
            {
                currentFocus = null;
                OnFocusExit?.Invoke();
            }
        }

        private void HandleInteraction()
        {
            if (currentFocus == null) return;

            switch (currentFocus.InteractionType)
            {
                case InteractionType.Instant:
                    if (IsInteractPressed())
                    {
                        if (!hasInteracted) // Only interact if we haven't already
                        {
                            ExecuteInteraction(currentFocus);
                            hasInteracted = true; // Mark as interacted
                        }
                    }
                    else
                    {
                        hasInteracted = false; // Reset when button is released
                    }
                    break;
                case InteractionType.Hold:
                    if (IsInteractPressed())
                    {
                        if (!isHolding)
                        {
                            isHolding = true;
                            interactionHoldTime = 0f;
                        }

                        interactionHoldTime += Time.deltaTime;
                        float progress = Mathf.Clamp01(interactionHoldTime / currentFocus.HoldDuration);

                        // Update the interaction progress
                        currentFocus.OnInteractionUpdate(progress);
                        OnHoldProgress?.Invoke(progress);

                        if (progress >= 1.0f)
                        {
                            ExecuteInteraction(currentFocus);
                            isHolding = false;
                        }
                    }
                    else if (isHolding)
                    {
                        // Reset if player released the button
                        isHolding = false;
                        OnHoldProgress?.Invoke(0f);
                    }
                    break;
            }
        }

        private readonly List<InputDevice> rightControllers = new List<InputDevice>();

        /// <summary>
        /// Interact is pressed if the Input System action fires OR the right
        /// controller's index trigger is pressed (read via XR InputDevices, the
        /// reliable OpenXR path on the Meta rig).
        /// </summary>
        private bool IsInteractPressed()
        {
            if (inputManager != null && inputManager.Interact)
                return true;

            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, rightControllers);
            foreach (var device in rightControllers)
            {
                if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool pressed) && pressed)
                    return true;
            }
            return false;
        }

        private void ExecuteInteraction(IInteractable interactable)
        {
            var interactableToUse = interactable;

            currentFocus = null;
            OnFocusExit?.Invoke();

            interactableToUse.Interact();
        }
    }
}