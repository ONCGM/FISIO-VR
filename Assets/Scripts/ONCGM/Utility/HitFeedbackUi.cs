using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ONCGM.Utility {
    public class HitFeedbackUi : MonoBehaviour
    {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color failureColor = Color.red;
        [SerializeField] private List<string> successLines = new List<string>() { "Boa!", "Parabéns", "Foi!"};
        [SerializeField] private List<string> failureLines = new List<string>() { "Errou!", "Nope", "Quase lá"};
        [SerializeField] private float animationScale = 10f;
        [SerializeField] private float animationTime = 2f;

        // Variables.
        private WaitUntil waitUntil;
        private float lerp = 1f;
        private TMP_Text displayText;
        private ParticleSystem particles;
        #pragma warning restore 0649
        
        /// <summary>
        /// Starts a timeout and some lerp animations.
        /// </summary>
        private void Awake() {
            particles = GetComponentInChildren<ParticleSystem>();
            transform.LookAt(GameObject.FindWithTag("MainCamera").transform);
            displayText = GetComponentInChildren<TMP_Text>();
            transform.DOMoveY(transform.position.y + animationScale, animationTime, false);
            DOVirtual.Float(1f, 0f, animationTime, value => lerp = value);
            waitUntil = new WaitUntil(()=> lerp <= 0f);
            StartCoroutine(nameof(DestroyAfterLerp));
        }

        /// <summary>
        /// Picks a random text and sets from a text pool and sets the UI to that value.
        /// </summary>
        /// <param name="success"> Pick from success or failure pool. True for success pool.</param>
        public void PickRandomText(bool success) {
            displayText.text = ( success ? successLines[Random.Range(0, successLines.Count)] : failureLines[Random.Range(0, failureLines.Count)]);
            displayText.color = ( success ? successColor : failureColor);
            EmitParticles(success);
        }

        /// <summary>
        /// Emits the particles and changes their color based on if the player was successful or not.
        /// </summary>
        /// <param name="success"> Pick a color based on player result. True for success (uses same color as the text).</param>
        private void EmitParticles(bool success) {
            var particlesMain = particles.main;
            particlesMain.startColor = success ? successColor : failureColor;
            particles.Play();
        }

        /// <summary>
        /// Updates lerp values on target text.
        /// </summary>
        private void Update() {
            displayText.alpha = lerp;
        }

        /// <summary>
        /// Waits until animations are finished, then destroys itself.
        /// </summary>
        private IEnumerator DestroyAfterLerp() {
            yield return waitUntil;
            Destroy(gameObject);
        }
    }
}
