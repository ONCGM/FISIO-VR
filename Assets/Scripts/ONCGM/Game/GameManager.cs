using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ONCGM.Utility;
using ONCGM.VR.VREnums;
using UnityEngine.Events;

namespace ONCGM.Game {
    /// <summary>
    /// Manages the core of the game. I.e. loading scenes and other important stuff.
    /// </summary>
    public static class GameManager {
        static GameManager() {
            SceneManager.sceneLoaded += AllowSceneLoading;
            OnPause = new UnityEvent();
            OnResume = new UnityEvent();
            GameObject loader = GameObject.CreatePrimitive(PrimitiveType.Quad);
            loader.transform.position = Vector3.up * 10000f;
            loader.AddComponent<SceneLoadHelper>();
            loader.name = "Load Helper";
            loadHelper = loader.GetComponent<SceneLoadHelper>();
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
        }
        
        /// <summary>
        /// Is the game currently paused.
        /// </summary>
        public static bool IsPaused { get; private set; } = false;
        
        // Variables.
        /// <summary>
        /// Currently active game settings.
        /// </summary>
        public static GameSettings CurrentSettings { get; set; } = new GameSettings();

        /// <summary>
        /// The minigame that the user is currently playing.
        /// </summary>
        public static Minigames CurrentMinigame { get; private set; } = Minigames.CatchGame;

        /// <summary>
        /// Used to avoid double loading a scene.
        /// </summary>
        private static bool CanLoadAnotherScene { get; set; } = true;

        /// <summary>
        /// The scene that is currently active.
        /// </summary>
        public static GameScene CurrentLoadedScene { get; private set; } = GameScene.InvalidScene;
        
        /// <summary>
        /// The scene that was loaded before the current scene.
        /// </summary>
        public static GameScene LastLoadedScene { get; private set; } = GameScene.InvalidScene;
        
        // Events.
        /// <summary>
        /// Triggered on game pause.
        /// </summary>
        public static UnityEvent OnPause;
        /// <summary>
        /// Triggered on game unpause.
        /// </summary>
        public static UnityEvent OnResume;
        
        // Variables.
        private static SceneLoadHelper loadHelper;
        
        /// <summary>
        /// Allows the class to load another scene.
        /// </summary>
        private static void AllowSceneLoading(Scene scene, LoadSceneMode mode) {
            LastLoadedScene = CurrentLoadedScene;
            CurrentLoadedScene = (GameScene) scene.buildIndex;
            SceneManager.SetActiveScene(scene);
            CanLoadAnotherScene = true;  
        } 
        
        /// <summary>
        /// Loads a specified scene. Also sets the CurrentMinigame property.
        /// </summary>
        /// <param name="scene"> Scene to be loaded. </param>
        /// <param name="loadMode"> Which mode to load the scene. </param>
        public static void LoadScene(GameScene scene, LoadSceneMode loadMode) {
            if(scene == GameScene.InvalidScene) return;
            if(!CanLoadAnotherScene) return;
            CanLoadAnotherScene = false;
            loadHelper.WaitForSceneToLoad(SceneManager.LoadSceneAsync((int) scene, loadMode));
            
            switch(scene) {
                case GameScene.CatchGame:
                    CurrentMinigame = Minigames.CatchGame;
                    break;
                case GameScene.InvalidScene:
                    CurrentMinigame = Minigames.Invalid;
                    break;
                case GameScene.Calibration:
                    CurrentMinigame = Minigames.Invalid;
                    break;
                case GameScene.MainMenu:
                    CurrentMinigame = Minigames.Invalid;
                    break;
                case GameScene.GameSetup:
                    CurrentMinigame = Minigames.Invalid;
                    break;
                case GameScene.ColorsGame:
                    CurrentMinigame = Minigames.ColorsGame;
                    break;
                case GameScene.FlyingGame:
                    CurrentMinigame = Minigames.FlyingGame;
                    break;
                default:
                    CurrentMinigame = Minigames.Invalid;
                    break;
            }
        }

        /// <summary>
        /// Unloads a specified scene.
        /// </summary>
        /// <param name="scene"> Scene to be loaded. </param>
        /// <param name="waitForSceneLoadEvent"> Wait for a scene load event? True for yes.</param>
        public static void UnloadScene(GameScene scene, bool waitForSceneLoadEvent = true) {
            if(scene == GameScene.InvalidScene) return;
            if(SceneManager.GetSceneByBuildIndex((int)scene).isLoaded && waitForSceneLoadEvent) {
                SceneManager.sceneLoaded += (loadedScene, mode) => SceneManager.UnloadSceneAsync((int) scene);
            } else {
                SceneManager.UnloadSceneAsync((int) scene);
            }
        }
        
        /// <summary>
        /// Pauses or resumes the game.
        /// </summary>
        /// <param name="pause"> True for pause, false for resume.</param>
        public static void PauseGame(bool pause) {
            IsPaused = pause;
            
            if(pause) {
                OnPause.Invoke();
            } else {
                OnResume.Invoke();
            }
            
            Time.timeScale = pause ? 0f : 1f;
        }
        
        /// <summary>
        /// Returns which scene to load given current minigame settings.
        /// </summary>
        public static GameScene MinigamesToLoad(Minigames currentMinigame = Minigames.Invalid) {
            if((currentMinigame == Minigames.Invalid || currentMinigame == Minigames.All) && CurrentSettings.MinigamesToIncludeInSession == Minigames.All) {
                return GameScene.CatchGame;
            }

            if((currentMinigame == Minigames.Invalid || currentMinigame == Minigames.All) && CurrentSettings.MinigamesToIncludeInSession != Minigames.CatchAndColors && 
               CurrentSettings.MinigamesToIncludeInSession != Minigames.CatchAndFlying &&
               CurrentSettings.MinigamesToIncludeInSession != Minigames.ColorsAndFlying) {
                return (GameScene) ((int) GameScene.CatchGame + (int) CurrentSettings.MinigamesToIncludeInSession);
            }
            
            if(currentMinigame == Minigames.Invalid || currentMinigame == Minigames.All) {
                return CurrentSettings.MinigamesToIncludeInSession != Minigames.ColorsAndFlying ? GameScene.CatchGame : GameScene.ColorsGame;
            }
            
            if(currentMinigame == Minigames.CatchGame) {
                return (int) CurrentSettings.MinigamesToIncludeInSession < 3 ? 
                    GameScene.GameSetup : CurrentSettings.MinigamesToIncludeInSession == Minigames.CatchAndColors ?
                        GameScene.ColorsGame : GameScene.FlyingGame;
            }

            if(currentMinigame == Minigames.ColorsGame) {
                return (int) CurrentSettings.MinigamesToIncludeInSession < 3 ?
                    GameScene.GameSetup : CurrentSettings.MinigamesToIncludeInSession == Minigames.CatchAndColors ?
                        GameScene.GameSetup : GameScene.FlyingGame;
            }

            return GameScene.GameSetup;
        }
    }
}
