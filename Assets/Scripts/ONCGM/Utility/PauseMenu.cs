using System;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ONCGM.Utility {
    /// <summary>
    /// Displays and controls the in game pause menu.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class PauseMenu : MonoBehaviour
    {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField] private GameScene menuScene = GameScene.MainMenu;
        
        // Variables.
        private Canvas pauseCanvas;
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up listeners for the pause events in the 'GameManager'.
        /// </summary>
        private void Awake() {
            GameManager.OnPause.AddListener(OpenPauseMenu);
            GameManager.OnResume.AddListener(ClosePauseMenu);
            pauseCanvas = GetComponent<Canvas>();
        }

        /// <summary>
        /// When the 'OnPause' event is triggered. Display pause menu.
        /// </summary>
        private void OpenPauseMenu() {
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    if(CatchMinigameController.HasStarted) {
                        pauseCanvas.enabled = true;
                    }
                    break;
                case Minigames.ColorsGame:
                    if(ColorsMinigameController.HasStarted) {
                        pauseCanvas.enabled = true;
                    }
                    break;
                case Minigames.FlyingGame:
                    // Add later.
                    break;
            }
        }

        /// <summary>
        /// When the 'OnResume' event is triggered. Hide pause menu.
        /// </summary>
        private void ClosePauseMenu() {
            UiAudioHandler.PlayClip(UiAudioClips.ClickOut);
            pauseCanvas.enabled = false;
        }
        
        /// <summary>
        /// Restarts the minigame.
        /// </summary>
        public void RestartGame() {
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            
            if((int) GameManager.CurrentMinigame < 3) {
                GameManager.LoadScene((GameScene) ((int) GameScene.CatchGame + (int) GameManager.CurrentMinigame), LoadSceneMode.Single);
            } else {
                GameManager.LoadScene(GameScene.GameSetup, LoadSceneMode.Single);
            }
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void BackToMenu() {
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
            
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            GameManager.LoadScene(menuScene, LoadSceneMode.Single);
        }
        
        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void ContinueGame() {
            UiAudioHandler.PlayClip(UiAudioClips.ClickOut);
            GameManager.PauseGame(!GameManager.IsPaused);
        }
    }
}
