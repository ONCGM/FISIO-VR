using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ONCGM;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using TMPro;

namespace ONCGM.Utility {
    /// <summary>
    /// Counts down from '3' and starts the game logic.
    /// </summary>
    public class GameCountdown : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField, Range(1, 5)] private int countdownTime = 3;
        [SerializeField] private string gameStartString = "Começou";
        [SerializeField, Range(0.05f, 0.8f)] private float animationTime = 0.25f;
        [SerializeField] private Vector3 animationScale = Vector3.one;
        
        // Variables.
        private float animationElasticity = 1f;
        private int animationVibrato = 1;
        private WaitForSecondsRealtime waitASecond;
        private WaitForFixedUpdate waitFixedUpdate;
        private Action gameControllerBeginFunction;
        
        // Components.
        private TMP_Text displayText;

        #pragma warning disable 0649
        
        /// <summary>
        /// Sets up the class variables.
        /// </summary>
        private void Awake() {
            waitASecond = new WaitForSecondsRealtime(1f);
            waitFixedUpdate = new WaitForFixedUpdate();
            displayText = GetComponentInChildren<TMP_Text>();
        }

        /// <summary>
        ///  Sets up the class variables and starts the countdown
        /// </summary>
        private void Start() {
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    gameControllerBeginFunction = GameObject.FindObjectOfType<CatchMinigameController>().BeginMinigame;
                    break;
                case Minigames.ColorsGame:
                    gameControllerBeginFunction = GameObject.FindObjectOfType<ColorsMinigameController>().BeginMinigame;
                    break;
                case Minigames.FlyingGame:
                    // Add later.
                    break;
            }
            
            if(countdownTime > 0) StartCoroutine(nameof(Countdown));
            UiAudioHandler.PlayClip(UiAudioClips.StartGame);
            AnimateUi();
        }
        
        
        /// <summary>
        /// Counts down to zero and calls a function every second it counts.
        /// </summary>
        private IEnumerator Countdown() {
            yield return waitASecond;
            countdownTime--;
            CheckCountdown();
            AnimateUi();
            if(countdownTime > 0) StartCoroutine(nameof(Countdown));
        }
    
        /// <summary>
        /// Animates the UI text.
        /// </summary>
        private void AnimateUi() {
            displayText.text = countdownTime >= 1 ? countdownTime.ToString() : gameStartString;
            displayText.transform.DOPunchScale(animationScale, animationTime, animationVibrato, animationElasticity);
        }

        /// <summary>
        /// Check countdown time left to start the game.
        /// </summary>
        private void CheckCountdown() {
            if(countdownTime > 0) return;
            StartCoroutine(nameof(FadeText));
        }
        
        /// <summary>
        /// Fades the UI text on game start.
        /// </summary>
        private IEnumerator FadeText() {
            yield return waitASecond;
            while(displayText.alpha > 0f) {
                displayText.alpha -= Time.deltaTime;
                yield return waitFixedUpdate;
            }
            
            StartMinigame();
        }

        /// <summary>
        /// Starts the relevant minigame.
        /// </summary>
        private void StartMinigame() {
            gameControllerBeginFunction();
            Destroy(gameObject);
        }
    }
}