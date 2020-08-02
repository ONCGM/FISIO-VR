using System.Collections;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using TMPro;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Counts down the time text in the game UI based on the respective 'GameController' event 'tick'.
    /// </summary>
    public class TimerCountdown : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField] private string gameTimeText = "Tempo: ";
        [SerializeField] private string endGameText = "Fim de jogo.";
        
        // Variables
        private TMP_Text timerText;
        private WaitForSeconds waitASecond;
        private WaitForFixedUpdate waitFixedUpdate;
        private bool endGameAudioPlaying;
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up the class and gets the listeners ready. WIP.
        /// </summary>
        private void Start() {
            waitASecond = new WaitForSeconds(1f);
            waitFixedUpdate = new WaitForFixedUpdate();
            timerText = GetComponent<TMP_Text>();
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.Tick.AddListener(Timer);
                    CatchMinigameController.OnMinigameBegun.AddListener(FadeInText);
                    CatchMinigameController.OnMinigameEnded.AddListener(EndGameText);
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.Tick.AddListener(Timer);
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
        private void Timer() {
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    if(CatchMinigameController.HasStarted) {
                        UpdateUI(CatchMinigameController.SecondsLeftInMinigame.ToString());
                        if(CatchMinigameController.SecondsLeftInMinigame <= 3 && !endGameAudioPlaying) {
                            endGameAudioPlaying = true;
                            UiAudioHandler.PlayClip(UiAudioClips.EndGame);
                        }
                    }
                    break;
                case Minigames.ColorsGame:
                    if(ColorsMinigameController.HasStarted) {
                        UpdateUI(ColorsMinigameController.SecondsLeftInMinigame.ToString());
                        if(ColorsMinigameController.SecondsLeftInMinigame <= 3 && !endGameAudioPlaying) {
                            endGameAudioPlaying = true;
                            UiAudioHandler.PlayClip(UiAudioClips.EndGame);
                        }
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
            timerText.text = endGameText;
        }
        
        /// <summary>
        /// Updates the UI that displays time text.
        /// </summary>
        private void UpdateUI(string text) {
            timerText.text = string.Concat(gameTimeText, text, "s");
        }
        
        /// <summary>
        /// Fades the UI text on game start.
        /// </summary>
        private IEnumerator FadeText() {
            yield return waitASecond;
            while(timerText.alpha < 1f) {
                timerText.alpha += Time.deltaTime;
                yield return waitFixedUpdate;
            }
        }
    }
}
