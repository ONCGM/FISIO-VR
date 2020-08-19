using System.Collections;
using System.Collections.Generic;
using ONCGM.Utility;
using ONCGM.VR.VREnums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ONCGM.VR.VRButtons {
    /// <summary>
    /// Based on ToggleButton class. Used to specifically toggle the games music on and off.
    /// </summary>
    public class ToggleSfx : ToggleButton {
        protected void Start() {
            textToDisplayBeforeState = "SFX: ";
            ToggleState = SaveSystem.LoadedData.sfxEnabled;
            ToggleState = true;
        }
        
        /// <summary>
        /// Executes when the button in clicked.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData) {
            if(!canClick) return;
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            onClickHandler.Invoke();
            ToggleState = SaveSystem.LoadedData.sfxEnabled;
            canClick = false;
            StartCoroutine(nameof(UnlockClick));
        }
    }
}