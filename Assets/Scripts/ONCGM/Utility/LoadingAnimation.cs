using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
// ReSharper disable Unity.InefficientMultiplicationOrder

namespace ONCGM.Utility {
    /// <summary>
    /// Animates a sprite in game world to show that the game is loading or doing something important.
    /// </summary>
    public class LoadingAnimation : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField, Range(0.01f, 2f)] private float animationFadeSpeed = 0.25f;
        [SerializeField, Range(0.01f, 10f)] private float animationSpeed = 0.5f;
        [SerializeField, Range(5f, 90f)] private float spriteSeparation = 0.2f;
        [SerializeField, Range(1, 26f)] private int amountOfSprites = 6;
        [SerializeField, Range(0.5f, 5f)] private float offsetFromCenter = 1f;
        [SerializeField] private Transform canvasTransform;
        [SerializeField] private string spritePrefabPath = "Prefabs/UI/Loading Sprite";
        [SerializeField] private GameObject spritePrefab;
        
        // Variables.
        private bool rotateParent = false;
        private List<LoadingSpriteFade> spriteFades = new List<LoadingSpriteFade>();
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets the class up.
        /// </summary>
        private void Awake() {
            if(spritePrefab == null) {
                spritePrefab = Resources.Load<GameObject>(spritePrefabPath);
            }
        }
        
        /// <summary>
        /// Moves the animation around when enabled.
        /// </summary>
        private void FixedUpdate() {
            if(!rotateParent) return;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                                                 Quaternion.Euler(canvasTransform.rotation.eulerAngles.x, canvasTransform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + animationSpeed),
                                                 Mathf.InverseLerp(0f, 10f, animationSpeed));
        }

        /// <summary>
        /// Starts the loading animation.
        /// </summary>
        [ContextMenu("Start Animation")]
        public void StartAnimation() {
            rotateParent = true;
            for(int i = 0; i < amountOfSprites; i++) {
                LoadingSpriteFade sprite = Instantiate(spritePrefab, FindPoint(transform.position, offsetFromCenter, i), Quaternion.identity, transform).GetComponent<LoadingSpriteFade>();
                sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, sprite.transform.localPosition.y, 0f);
                sprite.transform.localRotation = Quaternion.Euler(Vector3.zero);
                sprite.MaxAlpha = Mathf.InverseLerp(0, amountOfSprites, i);
                sprite.FadeAnimationSpeed = animationFadeSpeed;
                sprite.StartCoroutine(nameof(LoadingSpriteFade.FadeIn));
                spriteFades.Add(sprite);
            }
        }
        
        /// <summary>
        /// Returns a point on unit sphere. Source: robertbu @ https://answers.unity.com/questions/750049/calculate-points-on-a-circle-sphere.html
        /// </summary>
        private Vector3 FindPoint(Vector3 center, float radius, int multiple) {
            return center + Quaternion.AngleAxis(spriteSeparation * multiple, Vector3.right) * (Vector3.up * radius);
        }
        
        /// <summary>
        /// Stops the loading animation.
        /// </summary>
        public void StopAnimation() {
            foreach(LoadingSpriteFade sprite in spriteFades) {
                sprite.StartCoroutine(nameof(LoadingSpriteFade.FadeOut));
            }
            spriteFades.Clear();
            canvasTransform.gameObject.GetComponent<Canvas>().enabled = false;
        }
    }
}