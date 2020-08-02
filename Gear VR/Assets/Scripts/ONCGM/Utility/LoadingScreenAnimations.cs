using System;
using System.Collections;
using System.Collections.Generic;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ONCGM.Utility {
    /// <summary>
    /// Animates the initial intro scene. Displays loading animation and initial sequence.
    /// </summary>
    public class LoadingScreenAnimations : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField] private string titleText = "<size=2>GEAR VR</size>";
        [SerializeField] private string workInProgressText = "Em desenvolvimento";
        [SerializeField] private string versionNameText = "Versão atual: 0.4";
        [SerializeField] private bool workInProgress = true; 
        [SerializeField] private float fadeInAnimationSpeed = 2f;

        [Header("Components and Object References")] 
        [SerializeField] private Image feevaleLogo;
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private TMP_Text loadingDotsText;
        [SerializeField] private TMP_Text versionText;
        [SerializeField] private FadeSprite blackScreenSprite;
        
        // Fade in variables.
        private float alpha = 0f;
        private WaitForSecondsRealtime waitHalfASecond;
        private WaitForSecondsRealtime waitForSeconds;
        private WaitForFixedUpdate waitFixedUpdate;
        
        // Other Variables.
        private static bool loadingComplete;
        
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up the components and objects.
        /// </summary>
        private void Awake() {
            versionText.text = string.Concat(titleText, Environment.NewLine, (workInProgress ? workInProgressText : string.Empty), Environment.NewLine, versionNameText);
            waitHalfASecond = new WaitForSecondsRealtime(0.5f);
            waitForSeconds = new WaitForSecondsRealtime(3f);
            waitFixedUpdate = new WaitForFixedUpdate();
            StartCoroutine(FadeInOrOut(true));
            StartCoroutine(nameof(LoadingDotsAnimation));
        }
        
        /// <summary>
        /// Updates color values for the texts and images.
        /// </summary>
        private void FixedUpdate() {
            versionText.alpha = alpha;
            loadingText.alpha = alpha;
            loadingDotsText.alpha = alpha;
            feevaleLogo.material.color = new Color(1f, 1f, 1f, alpha);
        }
        
        /// <summary>
        /// Sets the finished loading flag to true and plays the game boot up animations.
        /// </summary>
        public static void LoadingCompleted() {
            loadingComplete = true;
        }
        
        /// <summary>
        /// Animates the loading dots until the scenes are fully loaded.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadingDotsAnimation() {
            while(!loadingComplete) {
                if(loadingDotsText.text.Length < 3) {
                    loadingDotsText.text += ".";
                } else {
                    loadingDotsText.text = string.Empty;
                }
                yield return waitHalfASecond;
            }

            loadingText.text = "Concluído";
            StartCoroutine(FadeInOrOut(false));
        }

        /// <summary>
        /// Interpolates a value for controlling multiple objects alpha values at once.
        /// </summary>
        /// <param name="fadeIn"> True to fade in, false for fade out.</param>
        private IEnumerator FadeInOrOut(bool fadeIn) {
            if(fadeIn) {
                while(alpha < 1f) {
                    alpha += fadeInAnimationSpeed * Time.deltaTime;
                    yield return waitFixedUpdate;
                }
            } else {
                blackScreenSprite.enabled = true;
                while(alpha > 0f) {
                    alpha -= fadeInAnimationSpeed * Time.deltaTime;
                    yield return waitFixedUpdate;
                }
                
                GameManager.LoadScene(GameScene.GameSetup, LoadSceneMode.Single);
                yield return waitForSeconds;
            }
        }
    }
}