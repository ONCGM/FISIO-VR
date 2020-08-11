using System;
using System.Collections;
using System.Collections.Generic;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ONCGM.VR.VRInput {
    /// <summary>
    /// Controls the behaviour of the GearVR back button.
    /// </summary>
    public class BackButtonHandler : MonoBehaviour {
        /// <summary>
        /// Waits for the back button press and calls the GoBack method.
        /// </summary>
        private void Update() {
            if(OVRInput.GetDown(OVRInput.Button.Back, OVRInput.Controller.All)) GoBack();
        }

        /// <summary>
        /// Behaves differently depending on the current loaded scene on 'GameManager' 'LastLoadedSceneVariable'.
        /// </summary>
        private void GoBack() {
            if(GameManager.CurrentLoadedScene == GameScene.InvalidScene) {
                if(!SceneManager.GetSceneByBuildIndex((int) GameScene.MainMenu).isLoaded) GameManager.LoadScene(GameScene.MainMenu, LoadSceneMode.Single);
                CheckForLoadedScenes();
            } else if(GameManager.CurrentLoadedScene == GameScene.CatchGame ||
                      GameManager.CurrentLoadedScene == GameScene.ColorsGame ||
                      GameManager.CurrentLoadedScene == GameScene.FlyingGame){
                GameManager.PauseGame(!GameManager.IsPaused);
            } else {
                GoBackToLastScene();
            }
        }

        /// <summary>
        /// Checks if any scene other than the scenery and main menu are loaded and unloads them.
        /// </summary>
        private void CheckForLoadedScenes() {
            GameManager.PauseGame(false);
            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
                if(i != (int) GameScene.MainMenu) {
                    GameManager.UnloadScene((GameScene) i);
                }
            }
        }
        
        /// <summary>
        /// Changes the button behaviour accordingly.
        /// </summary>
        private void GoBackToLastScene() {
            GameManager.PauseGame(false);
            if(GameManager.LastLoadedScene == GameScene.InvalidScene) {
                GameManager.LoadScene(GameManager.LastLoadedScene, LoadSceneMode.Single);
            } else {
                GameManager.LoadScene(GameScene.MainMenu, LoadSceneMode.Single);
            }
        }
    }
}