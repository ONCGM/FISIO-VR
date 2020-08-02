using ONCGM.VR.VREnums;

namespace ONCGM.Game {
    /// <summary>
    /// Holds info for all the game settings.
    /// </summary>
    public class GameSettings {
        // Limiting constant variables.
        // Minimum angle.
        public float AbsoluteMinimumAngle { get; private set; } = 5f;
        public float AbsoluteMaximumAngle { get; private set; } = 30f;

        // Minimum Validation Time
        public float AbsoluteMinimumTimeToValidateInput { get; private set; } = 0.25f;
        public float AbsoluteMaximumTimeToValidateInput { get; private set; } = 60f;

        // Session time.
        public float AbsoluteMinimumSessionTimeInMinutes { get; private set; } = 0.5f;
        public float AbsoluteMaximumSessionTimeInMinutes { get; private set; } = 30f;
        
        /// <summary>
        /// Private property for MinimumAngle.
        /// </summary>
        private float minimumAngle = 15f;

        /// <summary>
        /// Reflects the minimum angle that the player needs to tilt the device to count as an input.
        /// This will be set on the game setup scene.
        /// </summary>
        public float MinimumAngle {
            get => minimumAngle;
            set {
                if(value < AbsoluteMinimumAngle) {
                    minimumAngle = AbsoluteMinimumAngle;
                } else if(value > AbsoluteMaximumAngle) {
                    minimumAngle = AbsoluteMaximumAngle;
                } else {
                    minimumAngle = value;
                }
            }
        }
        
        /// <summary>
        /// Private property for MinimumTimeToValidateInput.
        /// </summary>
        private float minimumTimeToValidateInput = 1f;
        /// <summary>
        /// The amount of time that the player needs to be on the instructed input for it to be validated.
        /// This will be set on the game setup scene.
        /// </summary>
        public float MinimumTimeToValidateInput {
            get => minimumTimeToValidateInput;
            set {
                if(value < AbsoluteMinimumTimeToValidateInput) {
                    minimumTimeToValidateInput = AbsoluteMinimumTimeToValidateInput;
                } else if(value > AbsoluteMaximumTimeToValidateInput) {
                    minimumTimeToValidateInput = AbsoluteMaximumTimeToValidateInput;
                } else {
                    minimumTimeToValidateInput = value;
                }
            }
        }
        
        /// <summary>
        /// Private property for TotalSessionTime.
        /// </summary>
        private float totalSessionTime = 0.5f;
        /// <summary>
        /// The amount of time that the session will last in minutes. This will be set on the game setup scene.
        /// The minigames time will be equally divided from this value. 
        /// </summary>
        public float TotalSessionTime {
            get => totalSessionTime;
            set {
                if(value < AbsoluteMinimumSessionTimeInMinutes) {
                    totalSessionTime = AbsoluteMinimumSessionTimeInMinutes;
                } else if(value > AbsoluteMaximumSessionTimeInMinutes) {
                    totalSessionTime = AbsoluteMaximumSessionTimeInMinutes;
                } else {
                    totalSessionTime = value;
                }
            }
        }
        
        /// <summary>
        /// What difficulty should the game be. This will be set on the game setup scene.
        /// </summary>
        public GameDifficulty GameDifficulty { get; set; } = GameDifficulty.Normal;
        
        /// <summary>
        /// List of minigames to play in the session. These will be chosen on the game setup scene.
        /// </summary>
        public Minigames MinigamesToIncludeInSession { get; set; } = Minigames.CatchGame;

        /// <summary>
        /// What directions to use when spawning a object in the catch minigame.
        /// </summary>
        public SpawnDirection DirectionsToSpawnObjects { get; set; } = SpawnDirection.All;
        
        /// <summary>
        /// What type of objects to spawn in the catch game.
        /// </summary>
        public SpawnObjectsCatchGame TypesOfObjectsToSpawn { get; set; } = SpawnObjectsCatchGame.All;
    }
}