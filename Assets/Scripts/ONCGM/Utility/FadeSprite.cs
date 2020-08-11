using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ONCGM.Utility {
    /// <summary>
    /// Fades a sprite in or out then self destructs.
    /// </summary>
    public class FadeSprite : MonoBehaviour {
        [Header("Settings")] 
        [SerializeField] private bool fadeIn = true;
        [SerializeField] private bool selfDestruct = true;
        [SerializeField] private float fadeInAnimationSpeed = 0.25f;

        // Fade in variables.
        private float alpha = 0f;
        private WaitForFixedUpdate waitFixedUpdate;
        private SpriteRenderer sprite;

        public UnityAction OnFadeEnd { get; set; }

        /// <summary>
        /// Starts the coroutine to do the fade.
        /// </summary>
        private void Start() {
            waitFixedUpdate = new WaitForFixedUpdate();
            alpha = fadeIn ? 0f : 1f;
            StartCoroutine(FadeInOrOut(fadeIn));
            sprite = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Updates the alpha values on the components.
        /// </summary>
        private void Update() {
            sprite.color = new Color(0f, 0f, 0f, alpha);
        }

        /// <summary>
        /// Interpolates a value for controlling multiple objects alpha values at once.
        /// </summary>
        /// <param name="fade"> True to fade in, false for fade out.</param>
        private IEnumerator FadeInOrOut(bool fade) {
            if(fade) {
                while(alpha < 1f) {
                    alpha += fadeInAnimationSpeed * Time.deltaTime;
                    yield return waitFixedUpdate;
                }
            } else {
                while(alpha > 0f) {
                    alpha -= fadeInAnimationSpeed * Time.deltaTime;
                    yield return waitFixedUpdate;
                }
            }
            
            OnFadeEnd.Invoke();
            if(selfDestruct) Destroy(gameObject);
        }
    }
}