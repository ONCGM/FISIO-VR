using System.Collections;
using System.Collections.Generic;
using ONCGM.Game;
using ONCGM.Utility;
using ONCGM.VR.VRButtons;
using ONCGM.VR.VREnums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ONCGM.VR.VRInput {
    /// <summary>
    /// Used to toggle smooth input on or off.
    /// </summary>
    public class ToggleSmoothInput : ToggleButton {
        /// <summary>
        /// Updates text values.
        /// </summary>
        protected IEnumerator Start() {
            yield return new WaitForSeconds(1f);
            ToggleState = GameManager.CurrentSettings.useSmoothedInput;
            UpdateState();
        }
        
        /// <summary>
        /// Executes when the button in clicked.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData) {
            if(!canClick) return;
            ToggleState = !ToggleState;
            GameManager.CurrentSettings.useSmoothedInput = ToggleState;
            onClickHandler.Invoke();
            canClick = false;
            StartCoroutine(nameof(UnlockClick));
        }
    }
}