using ONCGM.VR.VRButtons;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ONCGM.VR.VRInput {
    /// <summary>
    /// Controls the behaviour of a single key of the VRKeyboard class.
    /// </summary>
    public class VRKeyboardButton : GameWorldButton {
        [Header("Settings")] 
        [SerializeField] private bool useString = false;
        [SerializeField] private KeyCode buttonKey = KeyCode.None;
        [SerializeField] private string buttonString = string.Empty;
        
        
        /// <summary>
        /// Overrides the pointer down event and triggers a key press.
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData) {
            PressKey();
        }


        /// <summary>
        /// Called by a Unity button. Used to register a key press by the user.
        /// </summary>
        public void PressKey() {
            if(useString) {
                VRKeyboard.StringButtonPressed.Invoke(buttonString);
            } else {
                VRKeyboard.ButtonPressed.Invoke(buttonKey);
            }
        }
    }
}
