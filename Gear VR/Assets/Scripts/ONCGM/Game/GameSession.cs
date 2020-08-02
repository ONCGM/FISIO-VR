using System;
using System.Collections.Generic;
using ONCGM.VR.VREnums;

namespace ONCGM.Game {
    /// <summary>
    /// Holds information about a game session.
    /// </summary>
    [Serializable]
    public class GameSession {
        /// <summary>
        /// The name of the player who generated this session data.
        /// </summary>
        public string PlayerName = "Jogador";
        
        /// <summary>
        /// The age of the player who generated this session data.
        /// </summary>
        public int PlayerAge = 0;

        /// <summary>
        /// The minimum angle setting on the game settings class. Used by the game manager.
        /// </summary>
        public float MinimumAngleSelected = 0f;

        /// <summary>
        /// The minimum time to validate input setting on the game settings class. Used by the game manager.
        /// </summary>
        public float MinimumTimeToValidateInput = 0f;

        /// <summary>
        /// The game difficulty setting on the game settings class. Used by the game manager.
        /// </summary>
        public int GameDifficulty = 0;
        
        /// <summary>
        /// What minigame was this session relevant to.
        /// </summary>
        public Minigames MinigameOfThisSession = Minigames.All;
        
        /// <summary>
        /// The setting of the of included minigames on the game settings class. Used by the game manager.
        /// </summary>
        public Minigames MinigamesUsedInSession = Minigames.All;

        /// <summary>
        /// A list of every angle the player reached on any given input during a game session.
        /// </summary>
        public List<float> AngleOnEveryInput = new List<float>();
        
        /// <summary>
        /// A list of every angle the player reached on any given input during a game session.
        /// </summary>
        public List<string> DirectionOnEveryInput = new List<string>();
        
        /// <summary>
        /// List of the position that every object spawned.
        /// </summary>
        public List<string> PositionOfEveryObjectSpawned = new List<string>();
        
        /// <summary>
        /// The amount of times the player made an input in the session.
        /// Meaning the total amount of repetitions that the player performed.
        /// </summary>
        public int AmountOfSuccessfulInputsOnSession = 0;
        
        /// <summary>
        /// The amount of times the player made an input in the session.
        /// Meaning the total amount of repetitions that the player performed.
        /// </summary>
        public int AmountOfUnsuccessfulInputsOnSession = 0;
        
        /// <summary>
        /// The time and date of the session collected when it started.
        /// </summary>
        public DateTime TimeAtBeginningOfSession = new DateTime();
        
        /// <summary>
        /// How long the session lasted in minutes.
        /// </summary>
        public float TotalSessionTime = 0f;

        /// <summary>
        /// The score that the player reached in this game session.
        /// </summary>
        public float SessionScore = 0f;

        /// <summary>
        /// The average angle delta variation during the session.
        /// </summary>
        public float AngleDeltaAverage = 0f;
    }
}