using System;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace ONCGM.VR.VRInput {
    /// <summary>
    /// Attaches to a Unity TextField or TMP_TextField and adds a keyboard under it so text can be input in the Gear VR.
    /// </summary>
    [RequireComponent(typeof(OVRRaycaster), typeof(Canvas))]
    public class VRKeyboard : MonoBehaviour {
        /// <summary>
        /// The offset from the attached object position.
        /// </summary>
        public Vector3 KeyboardPositionOffset { get; set; } = Vector3.zero;
        
        /// <summary>
        /// The keyboard will automatically enable this object if it is not a null when it is destroyed.
        /// </summary>
        public GameObject objectToActivateOnDestroy { get; set; }
        
        // Properties.
        public string TypedText { get; set; } = string.Empty;

        public TMP_Text TextField {
            get => textField;
            set {
                textField = value;
                initialTextFieldValue = textField.text;
                SetKeyboardPosition();
            }
        }

        // Variables.
        private TMP_Text textField;
        private string initialTextFieldValue;

        // Components.
        private OVRRaycaster raycaster;
        private Canvas canvas;
        
        // Events.
        public static Action<KeyCode> ButtonPressed;
        public static Action<string> StringButtonPressed;
        public static Action<string> InputConfirmed;

        /// <summary>
        /// Sets up the class and gathers references.
        /// </summary>
        private void Awake() {
            canvas = GetComponent<Canvas>();
            raycaster = GetComponent<OVRRaycaster>();
            raycaster.pointer = GameObject.FindObjectOfType<OVRGazePointer>().gameObject;
            ButtonPressed = Type;
            StringButtonPressed = TypeString;

            if(raycaster.pointer == null) Debug.Log("Raycaster missing Gaze Pointer");
        }
        
        /// <summary>
        /// Sets the keyboard position to be in front of the text field.
        /// </summary>
        private void SetKeyboardPosition() {
            transform.position = textField.transform.position + KeyboardPositionOffset;
        }
        
        /// <summary>
        /// Types a key, or erases it if it's a backspace, in the attached text field.
        /// </summary>
        /// <param name="key"> Key to type or erase it if it's a backspace.</param>
        private void Type(KeyCode key) {
            if(key != KeyCode.Backspace && key != KeyCode.KeypadEnter && key != KeyCode.Return &&
               key != KeyCode.Escape && key != KeyCode.Space) {
                TypedText += key.ToString();
            } else if(key == KeyCode.Space) {
                TypedText += " ";
            } else if(key == KeyCode.Backspace) {
                TypedText = TypedText.Remove(TypedText.Length - 1);
            } else if(key == KeyCode.Return || key == KeyCode.KeypadEnter) {
                InputConfirmed.Invoke(TypedText);
                CloseKeyboard();
            } else if(key == KeyCode.Escape) {
                CloseKeyboard();
            } 
            
            if(TextField == null) return;
            TextField.text = TypedText;
        }
        
        /// <summary>
        /// Types a number (only) in the attached text field.
        /// </summary>
        /// <param name="key"> The string to type in. </param>
        private void TypeString(string key) {
            TypedText += key;
            
            if(TextField == null) return;
            TextField.text = TypedText;
        }
        
        /// <summary>
        /// If escape was pressed, destroys the keyboard.
        /// </summary>
        private void CloseKeyboard() {
            if(TextField != null) TextField.text = initialTextFieldValue;
            Destroy(gameObject);
        }

        /// <summary>
        /// Activates the object in the 'objectToActivateOnDestroy' variable.
        /// </summary>
        private void OnDestroy() {
            if(objectToActivateOnDestroy != null) objectToActivateOnDestroy.SetActive(true); 
        }
    }
}
