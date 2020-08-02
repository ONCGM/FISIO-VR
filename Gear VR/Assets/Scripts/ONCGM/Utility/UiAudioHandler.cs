    using System;
    using System.Collections;
using System.Collections.Generic;
using ONCGM.VR.VREnums;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Plays any and all UI sounds. To play a sound just call the 'PlayClip' function and pass the desired audio clip along. 
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class UiAudioHandler : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Audio clips MUST be in the same order as the 'UiAudioClips' enum.")]
        [SerializeField] private AudioClip[] audioClips;
    
        // Variables.
        private static AudioSource aSource;
        private static AudioClip[] aClips;
        private static UiAudioClips lastPlayedClip;

        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up the class and checks if there are duplicates of this class.
        /// </summary>
        private void Awake() {
            if(GameObject.FindObjectsOfType<UiAudioHandler>().Length > 1) {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            
            aSource = GetComponent<AudioSource>();
            aClips = audioClips;
        }
        
        /// <summary>
        /// Resets the 'lastPlayedClip' to a neutral value to allow new sounds to be played and to avoid doubling the sound effects by the ovr input.
        /// </summary>
        private void LateUpdate() {
            lastPlayedClip = UiAudioClips.None;
        }

        /// <summary>
        /// Plays the specified audio clip.
        /// </summary>
        /// <param name="clip"> What clip to use. </param>
        public static void PlayClip(UiAudioClips clip) {
            if(clip == lastPlayedClip) return;
            if(aSource != null) aSource.PlayOneShot(aClips[(int) clip]);
            lastPlayedClip = clip;
        }
    }
}