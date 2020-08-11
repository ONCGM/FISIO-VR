using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Used to control and randomize the dog barks in the game.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DogBarkAudioController : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField] private bool useRandomPitch = false;
        [SerializeField, Range(0f, 1f)] private float randomPitchVariation = 0.2f;
        [SerializeField, Range(5f, 30f)] private float minimumWaitTimeBetweenLoops = 10f;
        [SerializeField, Range(0f, 10f)] private float randomWaitTimeVariation = 5f;
        [SerializeField] private Transform[] transformsToUseAsRandomPosition;
        
        // Variables
        private AudioSource aSource;
        private WaitForSeconds waitForSeconds;
        
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up the class variables.
        /// </summary>
        private void Awake() {
            aSource = GetComponent<AudioSource>();
            PlayAudio();
        }

        /// <summary>
        /// Plays the audio clip and start a coroutine to play it again later.
        /// </summary>
        private void PlayAudio() {
            transform.position =
                transformsToUseAsRandomPosition[Random.Range(0, transformsToUseAsRandomPosition.Length)].position;
            var pitch = aSource.pitch;
            aSource.pitch = useRandomPitch ? Random.Range(pitch - randomPitchVariation, pitch + randomPitchVariation) : 1f;
            aSource.Play();
            var clip = aSource.clip;
            waitForSeconds = new WaitForSeconds(Random.Range(clip.length + minimumWaitTimeBetweenLoops, clip.length + minimumWaitTimeBetweenLoops + randomWaitTimeVariation));
            StartCoroutine(nameof(WaitBetweenLoops));
        }

        /// <summary>
        /// Waits for X amount of seconds and calls the play audio function so the loops can continue.
        /// </summary>
        private IEnumerator WaitBetweenLoops() {
            yield return waitForSeconds;
            PlayAudio();
        }
    }
}