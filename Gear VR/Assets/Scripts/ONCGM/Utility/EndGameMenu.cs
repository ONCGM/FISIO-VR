using System.Globalization;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ONCGM.Utility {
    /// <summary>
    /// Displays the end game results and some menu options.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class EndGameMenu : MonoBehaviour
    {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField] private string timeEndText = "min";
        [SerializeField] private string scoreEndText = " Pontos";
        [SerializeField] private string inputsEndText = " Inputs";
        [SerializeField] private GameScene menuScene = GameScene.MainMenu;
        [SerializeField] private GameScene gameScene = GameScene.CatchGame;

        [Header("Components")] 
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text inputsText;
        [SerializeField] private Button nextMinigameButton;
        
        // Variables.
        private Canvas endGameCanvas;
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up listeners for the pause events in the 'GameManager'.
        /// </summary>
        private void Start() {
            endGameCanvas = GetComponent<Canvas>();

            if(GameManager.MinigamesToLoad(GameManager.CurrentMinigame) != GameScene.GameSetup) {
                nextMinigameButton.interactable = true;
            } else {
                nextMinigameButton.interactable = false;
            }
            
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.OnMinigameEnded.AddListener(UpdateUiToDisplayResults);
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.OnMinigameEnded.AddListener(UpdateUiToDisplayResults);
                    break;
                case Minigames.FlyingGame:
                    // Add later
                    break;
            }
        }
        
        /// <summary>
        /// Updates the UI to show the end game values.
        /// </summary>
        private void UpdateUiToDisplayResults() {
            endGameCanvas.enabled = true;
            if(timeText != null && scoreText != null && inputsText != null) { 
                switch(GameManager.CurrentMinigame) {
                    case Minigames.CatchGame:
                        timeText.text = $"{GameManager.CurrentSettings.TotalSessionTime.ToString(CultureInfo.InvariantCulture)} {timeEndText}";
                        scoreText.text = $"{CatchMinigameController.CurrentSession.SessionScore} {scoreEndText}";
                        inputsText.text = $"{CatchMinigameController.CurrentSession.AngleOnEveryInput.Count} {inputsEndText}";
                        break;
                    case Minigames.ColorsGame:
                        timeText.text = $"{GameManager.CurrentSettings.TotalSessionTime.ToString(CultureInfo.InvariantCulture)} {timeEndText}";
                        scoreText.text = $"{ColorsMinigameController.CurrentSession.SessionScore} {scoreEndText}";
                        inputsText.text = $"{ColorsMinigameController.CurrentSession.AngleOnEveryInput.Count} {inputsEndText}";
                        break;
                    case Minigames.FlyingGame:
                        // Add later
                        break;
                }
            } else {
                timeText.text = "Algo";
                scoreText.text = "deu";
                inputsText.text = "errado!";
            }
        }
        
        /// <summary>
        /// Restarts the minigame.
        /// </summary>
        public void NextMinigame() {
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            GameManager.LoadScene(GameManager.MinigamesToLoad(GameManager.CurrentMinigame), LoadSceneMode.Single);
        }
        
        /// <summary>
        /// Restarts the minigame.
        /// </summary>
        public void RestartGame() {
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            GameManager.LoadScene(gameScene, LoadSceneMode.Single);
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void BackToMenu() {
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.SaveSessionData();
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.SaveSessionData();
                    break;
                case Minigames.FlyingGame:
                    // Add later.
                    break;
            }
            
            GameManager.LoadScene(menuScene, LoadSceneMode.Single);
        }
    }
}
