using System.Collections;
using System.Collections.Generic;
using ONCGM.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ONCGM.VR.VRButtons {
    /// <summary>
    /// A toggle button based on a TMP_Text button and using DoTween for animations.
    /// Toggles seated position in serialization.
    /// Does not influence anything in gameplay.
    /// </summary>
    public class ToggleSeatedPosition : ToggleButton {
        /// <summary>
        /// Override pointer click to change the position setting.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData) {
            if(!canClick) return;
            onClickHandler.Invoke();
            ToggleState = !ToggleState;
            canClick = false;
            StartCoroutine(nameof(UnlockClick));
            SaveSystem.LoadedData.isStanding = ToggleState;
            SaveSystem.SaveGameToFile();
        }
    }
}