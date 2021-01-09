using System;
using System.Collections;
using System.Collections.Generic;
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