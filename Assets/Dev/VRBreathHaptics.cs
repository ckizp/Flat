using System.Collections.Generic;
using Flat.Gameplay.Characters;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Pulses a light haptic on both controllers in sync with the player's breathing
/// (<see cref="PlayerSound.OnExpire"/>). The pulse gets stronger and longer as the
/// player's anxiety rises (<see cref="PlayerAnxietyController.NormalizedAnxiety"/>).
/// </summary>
public class VRBreathHaptics : MonoBehaviour
{
    [Header("Amplitude (0-1)")]
    [SerializeField, Range(0f, 1f)] private float calmAmplitude = 0.08f;
    [SerializeField, Range(0f, 1f)] private float panicAmplitude = 0.35f;

    [Header("Duration (seconds)")]
    [SerializeField] private float calmDuration = 0.12f;
    [SerializeField] private float panicDuration = 0.22f;

    private readonly List<InputDevice> controllers = new List<InputDevice>();

    private void OnEnable()
    {
        PlayerSound.OnExpire += Pulse;
    }

    private void OnDisable()
    {
        PlayerSound.OnExpire -= Pulse;
    }

    private void Pulse()
    {
        float anxiety = PlayerAnxietyController.Instance != null
            ? PlayerAnxietyController.Instance.NormalizedAnxiety
            : 0f;

        float amplitude = Mathf.Lerp(calmAmplitude, panicAmplitude, anxiety);
        float duration = Mathf.Lerp(calmDuration, panicDuration, anxiety);

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, controllers);
        foreach (var device in controllers)
        {
            if (device.TryGetHapticCapabilities(out var caps) && caps.supportsImpulse)
                device.SendHapticImpulse(0u, amplitude, duration);
        }
    }
}
