using System;
using System.Security.Cryptography;
using ONCGM.VR.VRButtons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ONCGM.VR.VRInput {
    public class InstantiateVRKeyboard : GameWorldButton {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField] private Vector3 keyboardPositionOffset = Vector3.zero;
        [SerializeField] private bool useNumericalKeyboard = false;
        
        // Variables.
        private GameObject keyboardPrefab;
        private VRKeyboard keyboard;
        
        [Header("Components")]
        [SerializeField] private TMP_Text textField;
        [SerializeField] private GameObject deactivateObjectOnKeyboardOpen;
        
        #pragma warning restore 0649

        /// <summary>
        /// Sets up the class and instantiates the VR Keyboard.
        /// </summary>
        private void Awake() {
            keyboardPrefab = Resources.Load<GameObject>(useNumericalKeyboard ? "Prefabs/VR/Numerical VR Keyboard" : "Prefabs/VR/VR Keyboard");
        }

        /// <summary>
        /// Detects a click and instantiates the VRKeyboard on the attached image.
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData) {
            if(keyboard == null) {
                if(deactivateObjectOnKeyboardOpen != null) deactivateObjectOnKeyboardOpen.SetActive(false);
                keyboard = Instantiate(keyboardPrefab, transform).GetComponent<VRKeyboard>();
                keyboard.KeyboardPositionOffset = keyboardPositionOffset;
                keyboard.objectToActivateOnDestroy = deactivateObjectOnKeyboardOpen;
                keyboard.TextField = textField;
                keyboard.TypedText = textField.text;
            } else {
                if(deactivateObjectOnKeyboardOpen != null) deactivateObjectOnKeyboardOpen.SetActive(true);
                foreach(var key in GameObject.FindObjectsOfType<VRKeyboard>()) {
                    Destroy(key.gameObject);
                }
            }
        }
    }
}
