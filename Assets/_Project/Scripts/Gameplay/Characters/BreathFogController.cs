using UnityEngine;

namespace Flat.Gameplay.Characters
{
    public class BreathFogController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("The breath fog particle system")]
        private ParticleSystem breathParticles;

        [Header("Particle Settings")]
        [SerializeField, Tooltip("Base number of particles emitted per breath")]
        private int baseParticleCount = 5;

        [SerializeField, Tooltip("Extra particles at max anxiety")]
        private int panicExtraParticles = 10;

        private PlayerAnxietyController _anxietyController;

        private void OnEnable()
        {
            PlayerSound.OnExpire += EmitBreath;
        }

        private void OnDisable()
        {
            PlayerSound.OnExpire -= EmitBreath;
        }

        private void Start()
        {
            _anxietyController = PlayerAnxietyController.Instance;

            if (breathParticles == null)
            {
                Debug.LogError("BreathFogController: No particle system assigned!");
                enabled = false;
                return;
            }

            var emission = breathParticles.emission;
            emission.enabled = false;
        }

        private void EmitBreath()
        {
            if (breathParticles == null) return;

            float anxiety = _anxietyController != null ? _anxietyController.NormalizedAnxiety : 0f;
            int particleCount = baseParticleCount + Mathf.RoundToInt(panicExtraParticles * anxiety);

            breathParticles.Emit(particleCount);
        }

        public void ForceBreath()
        {
            EmitBreath();
        }
    }
}
