using UnityEngine;
using System;

namespace Flat.Gameplay.Characters
{
    /// Manages the player's anxiety level (0-100).
    /// Other systems can subscribe to anxiety changes or modify the anxiety value.
    public class PlayerAnxietyController : MonoBehaviour
    {
        public static PlayerAnxietyController Instance { get; private set; }

        [Header("Anxiety Settings")]
        [SerializeField, Range(0f, 100f)] 
        private float baseAnxiety = 0f;
        
        [SerializeField, Tooltip("How fast anxiety decreases when no stress sources are active")]
        private float anxietyDecayRate = 5f;
        
        [SerializeField, Tooltip("Smoothing speed for anxiety transitions")]
        private float anxietyLerpSpeed = 3f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        private float _currentAnxiety;
        private float _targetAnxiety;
        private float _temporaryAnxietyBonus;

        /// Current anxiety level (0-100), smoothed
        public float CurrentAnxiety => _currentAnxiety;

        /// Normalized anxiety value (0-1) for easy use with other systems
        public float NormalizedAnxiety => _currentAnxiety / 100f;

        /// Event fired when anxiety level changes significantly
        public event Action<float> OnAnxietyChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple PlayerAnxietyController instances found. Destroying duplicate.");
                Destroy(this);
                return;
            }
            Instance = this;

            _currentAnxiety = baseAnxiety;
            _targetAnxiety = baseAnxiety;
        }

        private void Update()
        {
            UpdateAnxiety();
        }

        private void UpdateAnxiety()
        {
            // Calculate target anxiety (base + temporary bonuses, clamped)
            _targetAnxiety = Mathf.Clamp(baseAnxiety + _temporaryAnxietyBonus, 0f, 100f);

            // Smoothly interpolate current anxiety towards target
            float previousAnxiety = _currentAnxiety;
            _currentAnxiety = Mathf.Lerp(_currentAnxiety, _targetAnxiety, anxietyLerpSpeed * Time.deltaTime);

            // Decay temporary anxiety bonus over time
            if (_temporaryAnxietyBonus > 0f)
            {
                _temporaryAnxietyBonus = Mathf.Max(0f, _temporaryAnxietyBonus - anxietyDecayRate * Time.deltaTime);
            }

            // Fire event if anxiety changed significantly (threshold to avoid spam)
            if (Mathf.Abs(_currentAnxiety - previousAnxiety) > 0.1f)
            {
                OnAnxietyChanged?.Invoke(_currentAnxiety);
            }
        }

        /// Sets the base anxiety level. Useful for area-based anxiety (e.g., dark rooms)
        public void SetBaseAnxiety(float value)
        {
            baseAnxiety = Mathf.Clamp(value, 0f, 100f);
        }

        /// Adds a temporary anxiety spike that will decay over time.
        /// Useful for jump scares, seeing the shadow, etc.
        public void AddAnxietySpike(float amount)
        {
            _temporaryAnxietyBonus = Mathf.Clamp(_temporaryAnxietyBonus + amount, 0f, 100f);
        }

        /// Immediately sets the anxiety to a specific value (bypasses smoothing for the target)
        public void SetAnxietyImmediate(float value)
        {
            _currentAnxiety = Mathf.Clamp(value, 0f, 100f);
            _targetAnxiety = _currentAnxiety;
            _temporaryAnxietyBonus = 0f;
            OnAnxietyChanged?.Invoke(_currentAnxiety);
        }

        /// Resets anxiety to base level
        public void ResetAnxiety()
        {
            _temporaryAnxietyBonus = 0f;
            _targetAnxiety = baseAnxiety;
        }

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 10, 200, 100));
            GUILayout.Label($"Anxiety: {_currentAnxiety:F1}/100");
            GUILayout.Label($"Base: {baseAnxiety:F1}");
            GUILayout.Label($"Temp Bonus: {_temporaryAnxietyBonus:F1}");
            GUILayout.EndArea();
        }
    }
}
