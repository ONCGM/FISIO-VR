using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using ONCGM.Game;
using ONCGM.Utility;
using ONCGM.VR.VREnums;

namespace ONCGM.VR.VRButtons {
    /// <summary>
    /// Toggles the minimum input time in the game setup scene. Inherits from 'GameWorldButton'.
    /// </summary>
    public class ToggleInputTime : GameWorldButton {
        [Header("OnClick Settings")]
        [SerializeField] protected OnClickEvent onClickHandler = new OnClickEvent();
        [Header("Text Settings")] 
        [SerializeField] protected string textToDisplayBeforeState = "Validação: ";
        [Header("Animation Settings")]
        [SerializeField] protected Vector3 animationStrength = Vector3.zero;
        [SerializeField, Range(0f, 1f)] protected float animationElasticity = 1f;
        [SerializeField, Range(0, 10)] protected int animationVibrato = 1;
        [SerializeField, Range(0.05f, 1f)] protected float animationDuration = 0.15f;
        
        // Variables.
        private WaitForEndOfFrame waitAFrame;
        private WaitForSecondsRealtime waitASecond;
        protected bool canClick = true;
        
        /// <summary>
        /// Private variable from 'ToggleState' property.
        /// </summary>
        private float toggleState = 1f;
        
        /// <summary>
        /// The state of the toggle button.
        /// </summary>
        public float ToggleState {
            get => toggleState;
            set {
                if(toggleState != value) {
                    UpdateState();
                }
                toggleState = value;
            }
        }

        // Components.
        protected Button uiButton;
        protected TMP_Text uiText;
        
        // Events.
        [Serializable] public class OnClickEvent : UnityEvent{} 
    
        /// <summary>
        /// Sets the class up and gathers references for the components.
        /// </summary>
        protected virtual void Awake() {
            uiButton = GetComponent<Button>();
            uiText = GetComponentInChildren<TMP_Text>() as TMP_Text;
            waitAFrame = new WaitForEndOfFrame();
            waitASecond = new WaitForSecondsRealtime(0.4f);
            ToggleState = GameManager.CurrentSettings.MinimumTimeToValidateInput;
            UpdateState();
        }

        /// <summary>
        /// Updates the state of the text being displayed.
        /// </summary>
        [ContextMenu("Update State")]
        protected virtual void UpdateState() {
            AnimateClick();
            uiText.text = string.Concat(textToDisplayBeforeState, Environment.NewLine, ToggleState.ToString(CultureInfo.InvariantCulture), "s");
        }
        
        /// <summary>
        /// Executes when the button in clicked.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData) {
            if(!canClick) return;
            UiAudioHandler.PlayClip(UiAudioClips.Click);
            onClickHandler.Invoke();
            ToggleState = GameManager.CurrentSettings.MinimumTimeToValidateInput;
            UpdateState();
            canClick = false;
            StartCoroutine(nameof(UnlockClick));
        }

        /// <summary>
        /// Animates the text within the button when clicked.
        /// </summary>
        protected virtual void AnimateClick() {
            uiText.DOKill();
            uiText.transform.DOPunchScale(animationStrength, animationDuration, animationVibrato, animationElasticity);
            StartCoroutine(nameof(ReturnToNormalScale));
        }
        
        /// <summary>
        /// Returns the object to normal scale after a button press.
        /// Used to fix a DoTween issue.
        /// </summary>
        private IEnumerator ReturnToNormalScale() {
            while(uiText.transform.localScale != Vector3.one) {
                uiText.transform.localScale = Vector3.Lerp(uiText.transform.localScale, Vector3.one, Time.deltaTime);
                yield return waitAFrame;
            }
        }
        
        /// <summary>
        /// Avoids a accidental double click.
        /// </summary>
        protected virtual IEnumerator UnlockClick() {
            yield return waitASecond;
            canClick = true;
        }
    }
}
