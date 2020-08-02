using System.Collections;
using System.Collections.Generic;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ONCGM.Utility {
    /// <summary>
    /// Controls the game music in all areas.
    /// </summary>
    public class MusicHandler : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField, Range(0.001f, 1f)] private float fadeSpeed = 0.01f;
        [SerializeField, Range(0.001f, 1f)] private float maxVolumeOnAudioSources = 0.8f; 
    
        [Header("Music Files")] 
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip gameMusic;
        
        // Variables
        private AudioSource[] audioSources;
        private WaitForFixedUpdate waitForUpdate;
        
        #pragma warning restore 0649
        
        // Sets up the class by gathering references and subscribing to events.
        private void Awake() {
            if(GameObject.FindObjectsOfType<MusicHandler>().Length > 1) {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            audioSources = GetComponents<AudioSource>();
            audioSources[0].clip = mainMenuMusic;
            audioSources[1].clip = gameMusic;
            waitForUpdate = new WaitForFixedUpdate();
            SceneManager.sceneLoaded += OnSceneChanged;
            StartCoroutine(CrossFadeAudioSources(audioSources[1], audioSources[0], maxVolumeOnAudioSources));
        }
        
        /// <summary>
        /// Called whenever a scene is loaded, used to change the music.
        /// </summary>
        private void OnSceneChanged(Scene scene, LoadSceneMode mode) {
            if(GameManager.CurrentLoadedScene == GameScene.CatchGame) {
                StartCoroutine(CrossFadeAudioSources(audioSources[0], audioSources[1], maxVolumeOnAudioSources));
            } else {
                StartCoroutine(CrossFadeAudioSources(audioSources[1], audioSources[0], maxVolumeOnAudioSources));
            }
        }

        /// <summary>
        /// Cross fades the specified audio sources.
        /// </summary>
        /// <param name="fadeOut"> The audio source to fade out. </param>
        /// <param name="fadeIn"> The audio source to fade in. </param>
        /// <param name="maxVolume"> Maximum volume on fade in audio source. </param>
        private IEnumerator CrossFadeAudioSources(AudioSource fadeOut, AudioSource fadeIn, float maxVolume = 1f) {
            if(!fadeIn.isPlaying) fadeIn.Play();
            
            while(fadeOut.volume > 0f || fadeIn.volume < maxVolume) {
                fadeOut.volume -= fadeSpeed * Time.deltaTime;
                
                if(fadeIn.volume < maxVolume) {
                    fadeIn.volume += fadeSpeed * Time.deltaTime;
                }

                yield return waitForUpdate;   
            }
            
            if(fadeOut.isPlaying) fadeOut.Stop();
        }
    }
}