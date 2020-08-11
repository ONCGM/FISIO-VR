using UnityEngine;
using UnityEngine.Audio;
using ONCGM.Game;
using ONCGM.Utility;
using ONCGM.VR.VREnums;
using OVR;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ONCGM.VR.VRButtons {
    /// <summary>
    /// Holds all menu button functions.
    /// </summary>
    public class MenuButtonFunctions : MonoBehaviour {
        // Variables.
        private const string MixerPath = "Settings/AudioMixer";
        
        #region Calibration
        /// <summary>
        /// Starts the calibration process.
        /// </summary>
        public void StartCalibration() {
            CalibrationSequence calibration = GameObject.FindObjectOfType<CalibrationSequence>();
            if(calibration != null) {
                calibration.StartCalibrating();
                UiAudioHandler.PlayClip(UiAudioClips.Click);
            } else {
                LoadCalibrationScene();
            }
        }

        /// <summary>
        /// Loads the calibration scene.
        /// </summary>
        [ContextMenu("Load Calibration")]
        public void LoadCalibrationScene() {
            GameManager.PauseGame(false);
            if(!SceneManager.GetSceneByBuildIndex((int) GameScene.Calibration).isLoaded) {
                GameManager.LoadScene(GameScene.Calibration, LoadSceneMode.Single);
            }
            
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            
            if(GameManager.LastLoadedScene != GameScene.Calibration) {
                GameManager.UnloadScene(GameManager.LastLoadedScene);
            }
        }
        #endregion
        
        #region Scenes & Loading
        
        /// <summary>
        /// Opens or closes the credits menu. Pass a true to open and a false to close.
        /// </summary>
        public void SwitchToCreditsMenu(bool open) {
            if(!SceneManager.GetSceneByBuildIndex((int) GameScene.MainMenu).isLoaded) return;
            if(open) {
                GameObject.Find("Canvas Credits").GetComponent<Canvas>().enabled = true;
                GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
                UiAudioHandler.PlayClip(UiAudioClips.Click);
            } else {
                GameObject.Find("Canvas Credits").GetComponent<Canvas>().enabled = false;
                GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
                UiAudioHandler.PlayClip(UiAudioClips.ClickOut);
            }
        }
        
        /// <summary>
        /// Loads the game scene.
        /// </summary>
        [ContextMenu("Load Game")]
        public void LoadGameScene() {
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            GameManager.PauseGame(false);
            GameManager.LoadScene(GameManager.MinigamesToLoad(), LoadSceneMode.Single);
        }
        
        /// <summary>
        /// Loads the game setup scene.
        /// </summary>
        [ContextMenu("Load Game Setup")]
        public void LoadGameSetupScene() {
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            GameManager.PauseGame(false);
            GameManager.LoadScene(GameScene.GameSetup, LoadSceneMode.Single);
        }
        
        /// <summary>
        /// Loads the game main menu.
        /// </summary>
        [ContextMenu("Load Main Menu")]
        public void LoadMainMenu() {
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            GameManager.LoadScene(GameScene.MainMenu, LoadSceneMode.Single);
        }
        #endregion
        
        #region Settings and Game Settings
        /// <summary>
        /// Toggles game music on and off. Mixer must have "Music" exposed float.
        /// </summary>
        public void ToggleMusic() {
            SaveSystem.LoadedData.musicEnabled = !SaveSystem.LoadedData.musicEnabled;
            SaveSystem.SaveGameToFile();
            var mixer = Resources.Load<AudioMixer>(MixerPath);
            if(SaveSystem.LoadedData.musicEnabled) {
                mixer.SetFloat("Music", 0f);
            } else {
                mixer.SetFloat("Music", -85f);
            }
        }
        
        /// <summary>
        /// Toggles game sfx and ui sounds on and off. Mixer must have "Sfx" exposed float.
        /// </summary>
        public void ToggleSfx() {
            SaveSystem.LoadedData.sfxEnabled = !SaveSystem.LoadedData.sfxEnabled;
            SaveSystem.SaveGameToFile();
            var mixer = Resources.Load<AudioMixer>(MixerPath);
            if(SaveSystem.LoadedData.sfxEnabled) {
                mixer.SetFloat("Sfx", 0f);
            } else {
                mixer.SetFloat("Sfx", -85f);
            }
        }

        /// <summary>
        /// Toggles the playing position to be seated or standing.
        /// </summary>
        public void TogglePlayingPosition() {
            //SaveSystem.LoadedData.isStanding = ;
        }
    
        /// <summary>
        /// Changes the game difficulty to the next state then starts again.
        /// </summary>
        public void ChangeGameDifficulty() {
             if(GameManager.CurrentSettings.GameDifficulty != GameDifficulty.Impossible) {
                 GameManager.CurrentSettings.GameDifficulty =
                     (GameDifficulty) GameManager.CurrentSettings.GameDifficulty + 1;
             } else {
                 GameManager.CurrentSettings.GameDifficulty = GameDifficulty.VeryEasy;
             }
        }
        
        /// <summary>
        /// Changes the minimum angle required to perform and input to the next state then starts again.
        /// </summary>
        public void ChangeMinimumAngle() {
            if(GameManager.CurrentSettings.MinimumAngle >= GameManager.CurrentSettings.AbsoluteMaximumAngle) {
                GameManager.CurrentSettings.MinimumAngle = GameManager.CurrentSettings.AbsoluteMinimumAngle;
            } else {
                GameManager.CurrentSettings.MinimumAngle += 5f;
            }
        }
        
        /// <summary>
        /// Changes the session time to the next state then starts again.
        /// </summary>
        public void ChangeSessionTime() {
            if(GameManager.CurrentSettings.TotalSessionTime >= GameManager.CurrentSettings.AbsoluteMaximumSessionTimeInMinutes) {
                GameManager.CurrentSettings.TotalSessionTime = GameManager.CurrentSettings.AbsoluteMinimumSessionTimeInMinutes;
            } else {
                GameManager.CurrentSettings.TotalSessionTime += (GameManager.CurrentSettings.TotalSessionTime < 2f ? 0.5f : 2f);
            }
        }
        
        /// <summary>
        /// Changes the minimum input time to the next state then starts again.
        /// </summary>
        public void ChangeMinimumInputTime() {
            if(GameManager.CurrentSettings.MinimumTimeToValidateInput >= GameManager.CurrentSettings.AbsoluteMaximumTimeToValidateInput) {
                GameManager.CurrentSettings.MinimumTimeToValidateInput = GameManager.CurrentSettings.AbsoluteMinimumTimeToValidateInput;
            } else {
                GameManager.CurrentSettings.MinimumTimeToValidateInput += 
                    (GameManager.CurrentSettings.MinimumTimeToValidateInput < 1f ? 0.25f : 
                        GameManager.CurrentSettings.MinimumTimeToValidateInput < 5f ? 1f : 5f);
            }
        }
        
        /// <summary>
        /// Changes the directions that are going to be used in the catch minigame to spawn objects.
        /// </summary>
        public void ChangeSpawnDirection() {
            if(GameManager.CurrentSettings.DirectionsToSpawnObjects != SpawnDirection.All) {
                GameManager.CurrentSettings.DirectionsToSpawnObjects =
                    (SpawnDirection) GameManager.CurrentSettings.DirectionsToSpawnObjects + 1;
            } else {
                GameManager.CurrentSettings.DirectionsToSpawnObjects = SpawnDirection.Up;
            }
        }
        
        /// <summary>
        /// Changes the types of objects that are going to be used in the catch minigame.
        /// </summary>
        public void ChangeTypeOfObject() {
            if(GameManager.CurrentSettings.TypesOfObjectsToSpawn != SpawnObjectsCatchGame.All) {
                GameManager.CurrentSettings.TypesOfObjectsToSpawn =
                    (SpawnObjectsCatchGame) GameManager.CurrentSettings.TypesOfObjectsToSpawn + 1;
            } else {
                GameManager.CurrentSettings.TypesOfObjectsToSpawn = SpawnObjectsCatchGame.Butterfly;
            }
        }
        
        /// <summary>
        /// Changes the types of objects that are going to be used in the catch minigame.
        /// </summary>
        public void ChangeMinigamesInSession() {
            if(GameManager.CurrentSettings.MinigamesToIncludeInSession != Minigames.All) {
                GameManager.CurrentSettings.MinigamesToIncludeInSession =
                    (Minigames) GameManager.CurrentSettings.MinigamesToIncludeInSession + 1;
            } else {
                GameManager.CurrentSettings.MinigamesToIncludeInSession = Minigames.CatchGame;
            }
        }
        
        
        #endregion
    }
}
