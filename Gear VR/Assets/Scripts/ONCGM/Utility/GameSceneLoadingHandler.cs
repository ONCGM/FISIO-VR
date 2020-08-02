using System.Collections;
using System.Collections.Generic;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ONCGM.Utility {
    /// <summary>
    /// Controls the initial game scene loading and loads the game main scenery.
    /// </summary>
    public class GameSceneLoadingHandler : MonoBehaviour {
        /// <summary>
        /// Starts loading the game scenery, and when done, switches scene to the calibration one.
        /// </summary>
        private void Start() {
            Invoke(nameof(StartLoading), 5f);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Used to delay the loading progress to human readable speed.
        /// </summary>
        private void StartLoading() {
            GameManager.LoadScene(GameScene.CatchGame, LoadSceneMode.Single);
        }
        
        /// <summary>
        /// Starts loading the calibration scene after the scenery one.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            if(scene.buildIndex == (int) GameScene.CatchGame) {
                LoadingScreenAnimations.LoadingCompleted();
            }
        }
    }
}