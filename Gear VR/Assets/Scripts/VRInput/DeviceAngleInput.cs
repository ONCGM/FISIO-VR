using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using VREnums;
using Random = UnityEngine.Random;

// Used for enabling VRInput detection and making the access to the game custom input method.
namespace VRInput {
    /// <summary>
    /// Detects mobile phone acceleration and rotation and turns it into a detectable input.
    /// </summary>
    public class DeviceAngleInput : MonoBehaviour {
        [Header("Settings")]
        [Tooltip("Should the sensors detection be smoothed? True for yes.")]
        [SerializeField] private bool smoothDetection = false;
        [Tooltip("Defines how much the player has to lean in order to count as a input.")]
        [SerializeField, Range(5f, 35f)] private float minimumAngle = 10f;
        [Tooltip("How many samples should the centering system use to average values?")]
        [SerializeField, Range(10, 1000)] private int accelerationSamplesToCollect = 125;
        

        // Variables
        // Should we be detecting the player movement to determine which direction the player is leaning towards.
        private bool detectMovementInput = false;
        
        /// <summary>
        /// Used to dynamically call the right type of movement detection.
        /// </summary>
        private Action movementDetectionType = null;
        
        // True if the device has a gyroscope;
        private bool hasGyro = false;
        
        // True if the device has an accelerometer.
        private bool hasAccelerometer = false;
        
        // True if the system is trying to recenter and waiting for user confirmation.
        private bool waitForRecenterTap = false;

        // Values for calibrating the game.
        // Center rotation to be used as a reference point.
        private Quaternion centerRotation = Quaternion.identity;
        
        // The calculated average acceleration of the device at a neutral position.
        private Vector3 averagedAcceleration = Vector3.zero;
        // The acceleration of the device after subtracting the averaged acceleration.  
        public Vector3 calibratedAcceleration = Vector3.zero;
        // List of samples collected to average the device acceleration.
        private readonly List<Vector3> accelerationSamples = new List<Vector3>();
        // Wait a frame used in the Collect Samples coroutine.
        private readonly WaitForEndOfFrame waitAFrame = new WaitForEndOfFrame();

        /// <summary>
        /// Time it will take to recenter the device successfully.
        /// </summary>
        public static float TimeForRecenteringProcess { get; private set; } = 3f;
        
        // Adds some extra time in the recentering process. Must not be larger than 1.
        private const float recenteringExtraTimeModifier = 0.33f;
        
        // Current direction the player is leaning towards.
        private InputDirection currentDirection = InputDirection.InvalidDirection;

        /// <summary>
        /// Current direction the player is leaning towards.
        /// </summary>
        public InputDirection CurrentDirection {
            get => currentDirection;
            private set => currentDirection = value;
        }

        #region Unity Events
        /// <summary>
        /// Sets up the class to detect the needed values and use the phone gyro and compass.
        /// </summary>
        private void Awake() {
            hasAccelerometer = SystemInfo.supportsAccelerometer;
            hasGyro = SystemInfo.supportsGyroscope;
            TimeForRecenteringProcess = Mathf.Ceil((accelerationSamplesToCollect + (accelerationSamplesToCollect * 
                                        Mathf.Clamp01(recenteringExtraTimeModifier))) * Time.fixedDeltaTime);
            Input.compass.enabled = true;
            Input.gyro.enabled = true;
            Input.compensateSensors = smoothDetection;
            Input.location.Start();
        }
        
        /// <summary>
        ///  Automatically detects the first center point for tracking and sets up the detection type or displays error message if unsupported.
        /// </summary>
        private void Start() {
            centerRotation = Input.gyro.attitude;
            if(hasGyro && hasAccelerometer) {
                movementDetectionType = InputDetection;
            } else if(!hasGyro && hasAccelerometer) {
                DisplayUnsupportedWarningMessage();
            } else {
                DisplayUnsupportedWarningMessage();
            }
            
            StartTracking();
        }
        
        /// <summary>
        ///  Detects the recentering input from the player.
        /// </summary>
        private void Update() {
            if(!waitForRecenterTap) return;
            RecenterReference();
        }
        
        /// <summary>
        /// Detects input consistently together with the physics engines time.
        /// </summary>
        private void FixedUpdate() {
            if(!detectMovementInput) return;
            calibratedAcceleration = (averagedAcceleration - Input.acceleration);
            DetectPhoneRotation();
        }
        
        #endregion

        #region Recentering and tracking
        /// <summary>
        /// Allow the code to track the users movement.
        /// </summary>
        public void StartTracking() {
            detectMovementInput = true;
        }
        
        /// <summary>
        /// Initiates the recentering process, and lets the code wait for a tap on the touchpad.
        /// </summary>
        public void StartReferenceRecenter() {
            waitForRecenterTap = true;
        }
        
        /// <summary>
        /// Recenter the values of the reference point for tracking user movement and sets the needed values.
        /// </summary>
        private void RecenterReference() {
            if(!OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad)) return;
            ResetCalibration();
            waitForRecenterTap = false;
            centerRotation = Input.gyro.attitude;
            StartCoroutine(nameof(CollectAccelerationSamples));
            currentDirection = InputDirection.Centered;
        }
        
        /// <summary>
        /// Collects a set amount of acceleration samples from the device's accelerometer.
        /// </summary>
        private IEnumerator CollectAccelerationSamples() {
            while(accelerationSamples.Count < accelerationSamplesToCollect) {
                accelerationSamples.Add(Input.acceleration);
                yield return waitAFrame;
            }
            averagedAcceleration = new Vector3(accelerationSamples.Average(x=>x.x),
                                               accelerationSamples.Average(x=>x.y),
                                               accelerationSamples.Average(x=>x.z));
        }
        
        /// <summary>
        /// Resets the calibration of the device.
        /// </summary>
        public void ResetCalibration() {
            waitForRecenterTap = false;
            centerRotation = Quaternion.Euler(Vector3.zero);
            accelerationSamples.Clear();
            averagedAcceleration = Vector3.zero;
            calibratedAcceleration = Vector3.zero;
            currentDirection = InputDirection.InvalidDirection;
        }
        
        /// <summary>
        /// Calls the appropriated movement detection function via an action in case there are various implementations.
        /// </summary>
        private void DetectPhoneRotation() {
            movementDetectionType();
        }
        
        #endregion

        #region Movement Detection
        /// <summary>
        /// Uses the devices sensors to detect movement and set them as a input and sets the current direction enum accordingly.
        /// </summary>
        private void InputDetection() {
            currentDirection = CheckCenterTolerance() ? InputDirection.Centered : InputDirection.InvalidDirection;
            
            if(CheckAccelerometerTolerancePositive((smoothDetection ? calibratedAcceleration.y : Input.acceleration.y))) {
                currentDirection = InputDirection.Left;
            }
            
            if(CheckAccelerometerToleranceNegative((smoothDetection ? calibratedAcceleration.y : Input.acceleration.y))) {
                currentDirection = InputDirection.Right;
            }
            
            if(CheckAccelerometerTolerancePositive((smoothDetection ? calibratedAcceleration.z : Input.acceleration.z))) {
                currentDirection = InputDirection.Backward;
            }
            
            if(CheckAccelerometerToleranceNegative((smoothDetection ? calibratedAcceleration.z : Input.acceleration.z))) {
                currentDirection = InputDirection.Forward;
            } 
        }
        
        #endregion

        #region Math functions
        
        /// <summary>
        /// Checks if the received value is larger than the minimum value for counting as a input, used for positive values.
        /// </summary>
        /// <param name="value"> Desired angle to check. </param>
        /// <returns> Returns true if is larger than the tolerance. </returns>
        private bool CheckAccelerometerTolerancePositive(float value) => value > Mathf.InverseLerp(0f, 90f,minimumAngle);
        
        /// <summary>
        /// Checks if the received value is larger than the minimum value for counting as a input, used for negative values.
        /// </summary>
        /// <param name="value"> Desired angle to check. </param>
        /// <returns> Returns true if is larger than the tolerance. </returns>
        private bool CheckAccelerometerToleranceNegative(float value) => value < -Mathf.InverseLerp(0f, 90f,minimumAngle);

        /// <summary>
        /// Checks if the device is centered using all available sensors.
        /// </summary>
        /// <returns> Returns true if the device is centered </returns>
        private bool CheckCenterTolerance() {
            return !CheckAccelerometerTolerancePositive(Input.acceleration.y) &&
                   !CheckAccelerometerTolerancePositive(Input.acceleration.z) &&
                   Quaternion.Angle(centerRotation, Input.gyro.attitude) <= minimumAngle;
        }

        #endregion

        #region Utility Functions
        
        /// <summary>
        /// Triggers a error message if the device is not supported. 
        /// </summary>
        private void DisplayUnsupportedWarningMessage() {
            throw new NotImplementedException("This should display and error and then quit the app.");
        }

        #endregion
    }
}