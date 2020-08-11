using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Plays an audio clip when the 'PlayClip' function is called. Used by animation clips.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PlayAudioClipByAnimation : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Audio clip to play")]
        [SerializeField] private AudioClip aClip;
        [Header("Multiple audio clips to play (If there are clips here, it will pick one at random)")]
        [SerializeField] private AudioClip[] aClips;
        
        [Header("Settings")]
        [SerializeField] private bool playAsOneShot = false;
        [SerializeField] private bool useRandomPitch = false;
        [SerializeField, Range(0f, 1f)] private float pitchVariation = 0.15f;
        
        
        // Variables.
        private AudioSource aSource;
        
        #pragma warning restore 0649
        
        /// <summary>
        /// Gets the AudioSource component.
        /// </summary>
        private void Awake() {
            aSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays an audio clip on demand, the clip that is going to be used is set in the editor window.
        /// </summary>
        public void PlayClip() {
            aSource.clip = aClips.Length > 0 ? aClips[Random.Range(0, aClips.Length)] : aClip;

            var pitch = aSource.pitch;
            aSource.pitch = useRandomPitch ? Random.Range(pitch - pitchVariation, pitch + pitchVariation) : 1f;
            
            if(playAsOneShot) {
                aSource.PlayOneShot(aClips.Length > 0 ? aClips[Random.Range(0, aClips.Length)] : aClip);
            } else {
                aSource.Play();
            }
        }
    }
}