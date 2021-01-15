using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ONCGM.VR.VREnums;
using ONCGM.VR.VRInput;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Displays on a per-object basis how close the player is to
    /// getting to the desired position for the input.
    /// </summary>
    public class CatchGameProximityArrows : MonoBehaviour {
        [Header("Settings")] 
        [SerializeField] private Gradient proximityGradient = new Gradient();
        [SerializeField] private InputDirection directionToCheck = InputDirection.InvalidDirection;
        private bool display;
        private Vector2 desiredDirection = new Vector2();
        private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

        // Setup.
        private void Awake() {
            transform.GetChild(0).gameObject.SetActive(false);

            renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
            
            switch(directionToCheck) {
                case InputDirection.Forward:
                    desiredDirection = Vector2.down;
                    break;
                case InputDirection.Left:
                    desiredDirection = Vector2.right;
                    break;
                case InputDirection.Right:
                    desiredDirection = Vector2.left;
                    break;
                case InputDirection.Backward:
                    desiredDirection = Vector2.up;
                    break;
            }
        }

        // Updates the position of the arrows.
        private void Update() {
            // TODO: Disabled because it was not working. Needs fix.
            return;
            
            if(!display) return;
            var angle = Vector2.Angle(DeviceAngleInput.InputDirectionVector, desiredDirection);

            foreach(var spriteRenderer in renderers) {
                spriteRenderer.color = proximityGradient.Evaluate(Mathf.InverseLerp(0f, 180f, Mathf.Abs(angle)));
            }
            
            transform.GetChild(0).rotation = Quaternion.Euler(angle, 0f, 0f);
        }

        /// <summary>
        /// Starts displaying the proximity to the correct position.
        /// </summary>
        public void DisplayProximity() {
            transform.GetChild(0).gameObject.SetActive(true);
            display = true;
        }

        /// <summary>
        /// Stops displaying the arrows.
        /// </summary>
        public void StopDisplaying() {
            transform.GetChild(0).gameObject.SetActive(false);
            display = false;
        }
    }
}