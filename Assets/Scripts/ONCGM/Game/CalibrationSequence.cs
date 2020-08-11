using System;
using System.Collections;
using UnityEngine;
using TMPro;
using ONCGM.Utility;
using ONCGM.VR.VREnums;
using ONCGM.VR.VRInput;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Environment;

namespace ONCGM.Game {
    /// <summary>
    /// Handles the calibrating process.
    /// </summary>
    public class CalibrationSequence : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField, TextArea(1, 10)] private string idleText = "Por favor, olhe para o horizonte e se mantenha parado por um momento.";
        [SerializeField, TextArea(1, 10)] private string instructionText = "Calibrando, não se mova por favor.";
        [SerializeField, TextArea(1, 10)] private string etaText = "Tempo restante aproximado: ";
        [SerializeField, TextArea(1, 10)] private string loadingSceneText = "Dispositivo calibrado. Carregando Jogo.";
        [SerializeField, Range(0, 3)] private int spacingOnTextAfterStartingCalibration = 1;
        [Header("Components")] 
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private Button calibrateButton;
        [SerializeField] private Button backButton;
        [SerializeField] private LoadingAnimation loadingAnimation;
        
        // Variables & Components.
        private DeviceAngleInput deviceInput;
        private WaitForSecondsRealtime waitASecond;
        private VerticalLayoutGroup layoutGroup;
        private float timeLeft = 0;
        
        #pragma warning restore 0649
        /// <summary>
        /// Gather references.
        /// </summary>
        private void Start() {
            deviceInput = GameObject.FindObjectOfType<DeviceAngleInput>();
            waitASecond = new WaitForSecondsRealtime(1f);
            timeLeft = DeviceAngleInput.TimeForRecenteringProcess;
            DeviceAngleInput.OnRecenter.AddListener(EndCalibration);
            displayText.text = idleText;
            layoutGroup = GetComponent<VerticalLayoutGroup>();
        }
        
        /// <summary>
        /// Starts the calibration process and changes the UI to match.
        /// </summary>
        [ContextMenu("Calibrate")]
        public void StartCalibrating() {
            displayText.text = string.Concat(instructionText, NewLine, etaText, timeLeft);
            Destroy(calibrateButton.gameObject);
            Destroy(backButton.gameObject);
            layoutGroup.padding.top = spacingOnTextAfterStartingCalibration;
            loadingAnimation.StartAnimation();
            deviceInput.RecenterReference();
            StartCoroutine(nameof(UpdateUITimer));
        }
    
        /// <summary>
        /// Ends the calibration process and allows the player to go to the next scene.
        /// </summary>
        private void EndCalibration() {
            SaveSystem.LoadedData.TimesCalibrated++;
            SaveSystem.SaveGameToFile();
            displayText.text = loadingSceneText;
            loadingAnimation.StopAnimation();
            StopAllCoroutines();
            StartCoroutine(nameof(GoBackToLastScene));
        }
    
        /// <summary>
        /// Loads the scene that the user came from.
        /// </summary>
        private IEnumerator GoBackToLastScene() {
            GameManager.PauseGame(false);
            yield return waitASecond;
            yield return waitASecond;
            displayText.text = loadingSceneText;
            GameManager.LoadScene(GameManager.LastLoadedScene == GameScene.GameSetup ? 
                                      GameManager.LastLoadedScene : GameScene.MainMenu, LoadSceneMode.Single);

            yield return waitASecond;
        }
        
        
        /// <summary>
        /// Updates the UI timer during calibration.
        /// </summary>
        private IEnumerator UpdateUITimer() {
            yield return waitASecond;
            if(timeLeft < 0f) yield break;
            timeLeft -= 1f;
            displayText.text = string.Concat(instructionText, NewLine, etaText, Mathf.Clamp(timeLeft, 0f, 10f));
            StartCoroutine(nameof(UpdateUITimer));
        }
    }
}