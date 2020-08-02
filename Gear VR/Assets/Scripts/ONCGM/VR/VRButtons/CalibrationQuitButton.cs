using ONCGM.Game;
using ONCGM.Utility;
using ONCGM.VR.VREnums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ONCGM.VR.VRButtons {
    /// <summary>
    /// Changes the behaviour of the quit / back button depending on the scene.
    /// </summary>
    public class CalibrationQuitButton : MonoBehaviour {
        /// <summary>
        /// Changes the button accordingly.
        /// </summary>
        private void Start() {
            if(GameManager.LastLoadedScene == GameScene.InvalidScene) {
                GameManager.PauseGame(false);
                gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Changes the button behaviour accordingly.
        /// </summary>
        public void GoBackToLastScene() {
            UiAudioHandler.PlayClip(UiAudioClips.ClickOut);
            GameManager.PauseGame(false);
            if(GameManager.LastLoadedScene == GameScene.InvalidScene) {
                GameManager.LoadScene(GameManager.LastLoadedScene, LoadSceneMode.Single);
            } else {
                GameManager.LoadScene(GameScene.MainMenu, LoadSceneMode.Single);
            }
        }
    }
}