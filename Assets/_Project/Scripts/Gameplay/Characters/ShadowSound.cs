using System.Collections.Generic;
using UnityEngine;

namespace Flat.Gameplay.Characters
{
    public class ShadowSound : MonoBehaviour
    {
        [Header("Footsteps")]
        [SerializeField] private List<AudioClip> fs;

        private AudioSource footstepSource;

        void Start()
        {
            footstepSource = GetComponent<AudioSource>();
        }

        private void PlayFootstep()
        {
            AudioClip clip = fs[Random.Range(0, fs.Count)];

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