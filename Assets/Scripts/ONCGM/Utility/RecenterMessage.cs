using System;
using System.Collections;
using System.Collections.Generic;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using TMPro;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Displays a message to recalibrate in case the HMD loses focus.
    /// </summary>
    public class RecenterMessage : MonoBehaviour {
        #pragma warning disable 0649
        private TMP_Text textMesh;
        #pragma warning restore 0649
        
        // Setup.
        private void Awake() {
            textMesh = GetComponent<TMP_Text>();
            textMesh.alpha = 0f;
            OVRManager.HMDLost += DisplayMessage;
            OVRManager.VrFocusLost += DisplayMessage;

            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.Tick.AddListener(CheckErrorPercentageCatch);
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.Tick.AddListener(CheckErrorPercentageColors);
                    break;
            }
        }
        
        /// <summary>
        /// Checks to see if the player has made too many mistakes,
        /// and if so, suggests to recalibrate the game.
        /// </summary>
        private void CheckErrorPercentageCatch() {
            var totalInputs = CatchMinigameController.CurrentSession.QuantidadeDeAcertos +
                              CatchMinigameController.CurrentSession.QuantidadeDeErros;
            
            if(totalInputs < 5) return;

            if(totalInputs - CatchMinigameController.CurrentSession.QuantidadeDeErros < totalInputs * 0.6f) return;
            
            DisplayMessage();
        }
        
        /// <summary>
        /// Checks to see if the player has made too many mistakes,
        /// and if so, suggests to recalibrate the game.
        /// </summary>
        private void CheckErrorPercentageColors() {
            var totalInputs = ColorsMinigameController.CurrentSession.QuantidadeDeAcertos +
                              ColorsMinigameController.CurrentSession.QuantidadeDeErros;
            
            if(totalInputs < 5) return;

            if(totalInputs - ColorsMinigameController.CurrentSession.QuantidadeDeErros < totalInputs * 0.6f) return;
            
            DisplayMessage();
        }

        /// <summary>
        /// Shows the text in case the HMD was removed or lost focus.
        /// </summary>
        private void DisplayMessage() {
            textMesh.alpha = 1f;
        }

        // Unsubscribes from events.
        private void OnDestroy() {
            OVRManager.HMDLost -= DisplayMessage;
            OVRManager.VrFocusLost -= DisplayMessage;
        }
    }
}