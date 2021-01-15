using ONCGM.Game;
using ONCGM.VR.VRInput;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Indicates to the player were the centralization point is.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    public class CentralizationPointDisplayer : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField] private Transform playerHeadTransform;
        [SerializeField, Range(0.5f, 15f)] private float radiusToDisplayIcon = 7f;
        #pragma warning restore 0649

        // Setup.
        private void Awake() {
            // TODO: Disabled due to not being precise. New way of displaying info needed.
            // GameManager.OnPause.AddListener(ShowCentralizationPointIcon);
            // GameManager.OnResume.AddListener(HideCentralizationPointIcon);
            HideCentralizationPointIcon();
        }

        /// <summary>
        /// Displays and updates the position of the centralization icon.
        /// </summary>
        private void ShowCentralizationPointIcon() {
            var rotation = DeviceAngleInput.CenterRotation;
            var position = playerHeadTransform.position;
            
            var x = position.x + radiusToDisplayIcon * Mathf.Cos(rotation.x) * Mathf.Sin(rotation.y);
            var y = position.y + radiusToDisplayIcon * Mathf.Sin(rotation.x) * Mathf.Sin(rotation.y);
            var z = position.z + radiusToDisplayIcon * Mathf.Cos(rotation.y);
            
            var centralizationPointPosition = new Vector3(x, y, z);

            var childTransform = transform.GetChild(0);

            childTransform.position = centralizationPointPosition;
            childTransform.LookAt(playerHeadTransform);
            childTransform.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the centralization icon.
        /// </summary>
        private void HideCentralizationPointIcon() {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}