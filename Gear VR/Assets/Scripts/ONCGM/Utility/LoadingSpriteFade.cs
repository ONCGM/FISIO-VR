using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Fades the sprites on the loading animation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class LoadingSpriteFade : MonoBehaviour {
        // Variables.
        public float MaxAlpha { get; set; } = 0.5f;
        public float FadeAnimationSpeed { get; set; } = 0.05f;
        private Color color = Color.white;
        private WaitForEndOfFrame waitFrame;
        private SpriteRenderer spriteRenderer;

        /// <summary>
        /// Sets up the class and stuff.
        /// </summary>
        private void Awake() {
            waitFrame = new WaitForEndOfFrame();
            spriteRenderer = GetComponent<SpriteRenderer>();
            color.a = 0f;
            spriteRenderer.color = color;
        }

        /// <summary>
        /// Fades in the sprite to the desired alpha.
        /// </summary>
        public IEnumerator FadeIn() {
            color.a = 0f;
            spriteRenderer.color = color;
            while(spriteRenderer.color.a < MaxAlpha) {
                color.a += (FadeAnimationSpeed * Time.deltaTime);
                spriteRenderer.color = color;
                yield return waitFrame;
            }
        }
        
        /// <summary>
        /// Fades in the sprite to the desired alpha.
        /// </summary>
        public IEnumerator FadeOut() {
            while(spriteRenderer.color.a > 0f) {
                color.a -= (FadeAnimationSpeed * Time.deltaTime);
                spriteRenderer.color = color;
                yield return waitFrame;
            }
            Destroy(gameObject);
        }
    }
}