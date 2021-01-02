using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ONCGM.Game;
using UnityEngine;
using UnityEngine.Events;
using ONCGM.VR.VREnums;
using Random = UnityEngine.Random;


namespace ONCGM.VR.VRInput {
    /// <summary>
    /// Detects mobile phone acceleration and rotation and turns it into a detectable input.
    /// </summary>
    public class DeviceAngleInput : MonoBehaviour {
        // Use smoothing in calculations.
        private bool smoothDetection = true;
        
        // Minimum angle to count as input.
        private float minimumAngle = 5f;
        
        // How many samples to collect for smoothing.
        // Ideally, at least 60, but no more than 500, its unnecessary.
        private const int accelerationSamplesToCollect = 200;

        // Variables
        // Should we be detecting the player movement to determine which direction the player is leaning towards.
        private static bool detectMovementInput = false;
        
        /// <summary>
        /// Used to dynamically call the right type of movement detection.
        /// </summary>
        private static Action movementDetectionType = null;
        
        // True if the device has a gyroscope;
        private static bool hasGyro = false;
        
        // True if the device has an accelerometer.
        private static bool hasAccelerometer = false;

        // Values for calibrating the game.
        // Center rotation to be used as a reference point.
        private static Quaternion centerRotation = Quaternion.identity;
        
        /// <summary>
        /// The angle difference at any given time of the device compared to the centered position.
        /// </summary>
        public static float AngleDifferenceBetweenCenterPositionAndInput { get; private set; }= 0f;
        
        // The calculated average acceleration of the device at a neutral position.
        private static Vector3 averagedAcceleration = Vector3.zero;
        // The acceleration of the device after subtracting the averaged acceleration.  
        public static Vector3 CalibratedAcceleration { get; private set; }= Vector3.zero;
        // List of samples collected to average the device acceleration.
        private List<Vector3> accelerationSamples = new List<Vector3>();
        // Wait a frame used in the Collect Samples coroutine.
        private readonly WaitForEndOfFrame waitAFrame = new WaitForEndOfFrame();

        /// <summary>
        /// Time it will take to recenter the device successfully.
        /// </summary>
        public static float TimeForRecenteringProcess { get; private set; } = 3f;
        
        // Adds some extra time in the recentering process. Must not be larger than 1.
        private const float RecenteringExtraTimeModifier = 0.33f;
        
        // Current direction the player is leaning towards.
        private static InputDirection currentDirection = InputDirection.InvalidDirection;

        /// <summary>
        /// Current direction the player is leaning towards.
        /// </summary>
        public static InputDirection CurrentDirection {
            get => currentDirection;
            private set => currentDirection = value;
        }

        /// <summary>
        /// Used to calculate input validation.
        /// </summary>
        private float timeInCurrentInput = 0f;

        /// <summary>
        /// Used to stop the scrip from calculating multiple inputs on a frame.
        /// </summary>
        private bool alreadyCalculatedADirection = false;
        
        // Last detected direction.
        private static InputDirection lastDirection = InputDirection.InvalidDirection;
        
        // Events.
        /// <summary>
        /// Called whenever the device goes back to the center position.
        /// </summary>
        public static UnityEvent OnCentered;

        /// <summary>
        /// Called whenever the device reaches a new input position.
        /// </summary>
        public class OnInput : UnityEvent<InputDirection> { }
        
        /// <summary>
        /// Called whenever the device reaches a new input position.
        /// </summary>
        public static OnInput OnInputChange;
        
        /// <summary>
        /// Called whenever the device recenters to a new center point.
        /// </summary>
        public static UnityEvent OnRecenter;

        #region Unity Events
        /// <summary>
        /// Sets up the class to detect the needed values and use the phone gyro and compass.
        /// </summary>
        private void Awake() {
            GrabSettingsFromGameManager();
            hasAccelerometer = SystemInfo.supportsAccelerometer;
            hasGyro = SystemInfo.supportsGyroscope;
            TimeForRecenteringProcess = Mathf.Ceil((accelerationSamplesToCollect + (accelerationSamplesToCollect * 
                                        Mathf.Clamp01(RecenteringExtraTimeModifier))) * Time.fixedDeltaTime);
            Input.compass.enabled = true;
            Input.gyro.enabled = true;
            Input.compensateSensors = smoothDetection;
            Input.location.Start();
            OnCentered = new UnityEvent();
            OnInputChange = new OnInput();
            OnRecenter = new UnityEvent();
            OnInputChange.AddListener(AddInputAngleToSerialization);
        }
        
        /// <summary>
        /// Automatically detects the first center point for tracking and sets up the detection type or displays error message if unsupported.
        /// </summary>
        private void Start() {
            // Whenever doing a build after having implemented the closing dialog. uncomment lines in method.
            if(hasGyro && hasAccelerometer) {
                movementDetectionType = InputDetection;
            } else if(!hasGyro && hasAccelerometer) {
                movementDetectionType = InputDetection;
                //DisplayUnsupportedWarningMessage();
            } else {
                movementDetectionType = InputDetection;
                //DisplayUnsupportedWarningMessage();
            }

            GrabSettingsFromGameManager();
            StartTracking();
        }
        
        /// <summary>
        /// Detects input consistently together with the physics engines time.
        /// </summary>
        private void FixedUpdate() {
            if(!detectMovementInput) return;
            CalibratedAcceleration = (averagedAcceleration - Input.acceleration);
            movementDetectionType();

            // Override input on editor for testing purposes.
            if(Application.isEditor) EditorKeyboardInputOverride();
        }
        
        #endregion

        #region Recentering and tracking

        /// <summary>
        /// Updates settings to follow the ones the player has chosen.
        /// </summary>
        public void GrabSettingsFromGameManager() {
            minimumAngle = GameManager.CurrentSettings.MinimumAngle;
            smoothDetection = GameManager.CurrentSettings.useSmoothedInput;
        }
        
        /// <summary>
        /// Allow the code to track the users movement.
        /// </summary>
        public void StartTracking() {
            detectMovementInput = true;
        }
        
        /// <summary>
        /// Recenter the values of the reference point for tracking user movement and sets the needed values.
        /// </summary>
        public void RecenterReference() {
            ResetCalibration();
            StartCoroutine(nameof(CollectAccelerationSamples));
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
            centerRotation = Input.gyro.attitude;
            currentDirection = InputDirection.Centered;
            OnRecenter.Invoke();
        }
        
        /// <summary>
        /// Resets the calibration of the device.
        /// </summary>
        private void ResetCalibration() {
            centerRotation = Quaternion.Euler(Vector3.zero);
            accelerationSamples.Clear();
            averagedAcceleration = Vector3.zero;
            CalibratedAcceleration = Vector3.zero;
            currentDirection = InputDirection.InvalidDirection;
        }
        
        #endregion

        #region Movement Detection
        /// <summary>
        /// Uses the devices sensors to detect movement and set them as a input and sets the current direction enum accordingly.
        /// </summary>
        private void InputDetection() {
            AngleDifferenceBetweenCenterPositionAndInput = Quaternion.Angle(centerRotation, Input.gyro.attitude);
            
            if(CheckCenterTolerance() && !alreadyCalculatedADirection) {
                alreadyCalculatedADirection = true;
                currentDirection = InputDirection.Centered;
                timeInCurrentInput += 1f * Time.fixedDeltaTime;
                if(lastDirection != currentDirection && timeInCurrentInput >= GameManager.CurrentSettings.MinimumTimeToValidateInput) {
                    timeInCurrentInput = 0f;
                    OnCentered.Invoke();
                }
            } else {
                currentDirection = InputDirection.InvalidDirection;
            }

            if(CheckAccelerometerTolerancePositive((smoothDetection ? CalibratedAcceleration.y : Input.acceleration.y)) && !alreadyCalculatedADirection) {
                alreadyCalculatedADirection = true;
                timeInCurrentInput += 1f * Time.fixedDeltaTime;
                if(timeInCurrentInput >= GameManager.CurrentSettings.MinimumTimeToValidateInput) {
                    timeInCurrentInput = 0f;
                    currentDirection = InputDirection.Left;
                }
            }
            
            if(CheckAccelerometerToleranceNegative((smoothDetection ? CalibratedAcceleration.y : Input.acceleration.y)) && !alreadyCalculatedADirection) {
                alreadyCalculatedADirection = true;
                timeInCurrentInput += 1f * Time.fixedDeltaTime;
                if(timeInCurrentInput >= GameManager.CurrentSettings.MinimumTimeToValidateInput) {
                    timeInCurrentInput = 0f;
                    currentDirection = InputDirection.Right;
                }
            }
            
            if(CheckAccelerometerTolerancePositive((smoothDetection ? CalibratedAcceleration.z : Input.acceleration.z)) && !alreadyCalculatedADirection) {
                alreadyCalculatedADirection = true;
                timeInCurrentInput += 1f * Time.fixedDeltaTime;
                if(timeInCurrentInput >= GameManager.CurrentSettings.MinimumTimeToValidateInput) {
                    timeInCurrentInput = 0f;
                    currentDirection = InputDirection.Backward;
                }
            }
            
            if(CheckAccelerometerToleranceNegative((smoothDetection ? CalibratedAcceleration.z : Input.acceleration.z)) && !alreadyCalculatedADirection) {
                alreadyCalculatedADirection = true;
                timeInCurrentInput += 1f * Time.fixedDeltaTime;
                if(timeInCurrentInput >= GameManager.CurrentSettings.MinimumTimeToValidateInput) {
                    timeInCurrentInput = 0f;
                    currentDirection = InputDirection.Forward;
                }
            }
            
            if(lastDirection != currentDirection) {
                OnInputChange.Invoke(CurrentDirection);
            }
            
            lastDirection = currentDirection;
            alreadyCalculatedADirection = false;
        }

        /// <summary>
        /// Overrides input when using the editor for testing purposes.
        /// </summary>
        private static void EditorKeyboardInputOverride() {
            var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            // ReSharper disable once RedundantAssignment
            var inputDir = (input.x > 0.1f  ? InputDirection.Right :
                            input.x < -0.1f ? InputDirection.Left :
                                              (input.y > 0.1f  ? InputDirection.Backward :
                                               input.y < -0.1f ? InputDirection.Forward :
                                                                 InputDirection.Centered));

            if(inputDir != CurrentDirection) OnInputChange.Invoke(inputDir);
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
        /// Depending on the current minigame, send input data to the current session inside it.
        /// </summary>
        private void AddInputAngleToSerialization(InputDirection direction) {
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.CurrentSession.AnguloDeCadaMovimento.Add(AngleDifferenceBetweenCenterPositionAndInput);
                    CatchMinigameController.CurrentSession.DirecaoDeCadaMovimento.Add(InputDirectionExtension.ToString(direction));
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.CurrentSession.AnguloDeCadaMovimento.Add(AngleDifferenceBetweenCenterPositionAndInput);
                    ColorsMinigameController.CurrentSession.DirecaoDeCadaMovimento.Add(InputDirectionExtension.ToString(direction));
                    break;
                case Minigames.FlyingGame:
                    // Add later.
                    break;
            }
        }
        
        /// <summary>
        /// Triggers a error message if the device is not supported. 
        /// </summary>
        private void DisplayUnsupportedWarningMessage() {
            throw new NotImplementedException("This should display and error and then quit the app.");
        }

        #endregion
    }
}