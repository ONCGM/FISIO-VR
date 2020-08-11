using System;
using System.Collections;
using System.Globalization;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using ONCGM.VR.VRInput;
using TMPro;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Displays the game score on a text. Updated on demand.
    /// </summary>
    public class ScoreCounter : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private string scoreText = "Pontuação: ";
        [SerializeField] private string endGameText = "Pontuação Final: ";
        
        // Variables
        private TMP_Text scoreTextMesh;
        private WaitForSeconds waitASecond;
        private WaitForFixedUpdate waitFixedUpdate;
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up the class and gets the listeners ready. WIP.
        /// </summary>
        private void Start() {
            waitASecond = new WaitForSeconds(1f);
            waitFixedUpdate = new WaitForFixedUpdate();
            scoreTextMesh = GetComponent<TMP_Text>();
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.Tick.AddListener(Score);
                    CatchMinigameController.OnMinigameBegun.AddListener(FadeInText);
                    CatchMinigameController.OnMinigameEnded.AddListener(EndGameText);
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.Tick.AddListener(Score);
                    ColorsMinigameController.OnMinigameBegun.AddListener(FadeInText);
                    ColorsMinigameController.OnMinigameEnded.AddListener(EndGameText);
                    break;
                case Minigames.FlyingGame:
                    // Add later
                    break;
            }
        }
        
        /// <summary>
        /// Starts a coroutine to fade in the text.
        /// </summary>
        private void FadeInText() {
            StartCoroutine(nameof(FadeText));
        }
        
        /// <summary>
        /// Counts down the time text every second.
        /// </summary>
        private void Score() {
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    if(CatchMinigameController.HasStarted) {
                        UpdateUI(CatchMinigameController.CurrentSession.SessionScore.ToString(CultureInfo.CurrentCulture));
                    }
                    break;
                case Minigames.ColorsGame:
                    if(ColorsMinigameController.HasStarted) {
                        UpdateUI(ColorsMinigameController.CurrentSession.SessionScore.ToString(CultureInfo.CurrentCulture));
                    }
                    break;
                case Minigames.FlyingGame:
                    // Add later.
                    break;
            }
        }

        /// <summary>
        /// Updates the time text after the game has ended and displays relevant information.
        /// </summary>
        private void EndGameText() {
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    scoreTextMesh.text = string.Concat(endGameText, CatchMinigameController.CurrentSession.SessionScore);
                    break;
                case Minigames.ColorsGame:
                    scoreTextMesh.text = string.Concat(endGameText, ColorsMinigameController.CurrentSession.SessionScore);
                    break;
                case Minigames.FlyingGame:
                    // Add later
                    break;
            }
        }
        
        /// <summary>
        /// Updates the UI that displays time text.
        /// </summary>
        private void UpdateUI(string text) {
            scoreTextMesh.text = $"{scoreText}{text}";
            if(GameManager.CurrentMinigame == Minigames.ColorsGame) scoreTextMesh.text = $"Orientação: {InputDirectionExtension.ToString(DeviceAngleInput.CurrentDirection)}";
        }
        
        /// <summary>
        /// Fades the UI text on game start.    
        /// </summary>
        private IEnumerator FadeText() {
            yield return waitASecond;
            while(scoreTextMesh.alpha < 1f) {
                scoreTextMesh.alpha += Time.deltaTime;
                yield return waitFixedUpdate;
            }
        }
    }
}
