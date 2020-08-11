using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ONCGM.VR {
    /// <summary>
    /// Moves any object in the scene around the player transform, following its viewing direction.
    /// </summary>
    public class FollowCameraRotation : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")]
        [SerializeField, Range(0.01f, 10f)] private float lerpSpeed = 0.75f;
        [SerializeField, Range(1f, 10f)] private float offset = 5f;
        [Header("Components")] 
        [SerializeField] private Transform cameraTransform;
        #pragma warning restore 0649
        
        /// <summary>
        /// Updates the position of this object to be in front of the camera transform.
        /// </summary>
        // ReSharper disable Unity.InefficientPropertyAccess
        private void FixedUpdate() {
            if(cameraTransform == null) return;
            Vector3 position = Vector3.Lerp(transform.position, cameraTransform.forward * offset, lerpSpeed);
            transform.position = new Vector3(position.x, transform.position.y, position.z);
            transform.LookAt(cameraTransform);
        }
    }
}
