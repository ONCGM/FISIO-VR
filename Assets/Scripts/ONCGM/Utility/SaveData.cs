using System;
using System.Collections.Generic;
using ONCGM.Game;

namespace ONCGM.Utility {
    /// <summary>
    /// Class to hold game data and settings data.
    /// </summary>
    [Serializable]
    public class SaveData {
        // Save data.
        /// <summary>
        /// The player name.
        /// </summary>
        public string PlayerName = "Jogador";

        /// <summary>
        /// The age of the player.
        /// </summary>
        public int PlayerAge = 0;

        /// <summary>
        /// The patient medical id.
        /// </summary>
        public int PatientId = 0;
        
        /// <summary>
        /// Did the player stand for this session.
        /// </summary>
        public bool isStanding = false;

        // Stats data.
        /// <summary>
        /// How many times the player calibrated his device.
        /// </summary>
        public int TimesCalibrated = 0;
        
        /// <summary>
        /// The game session that this player generated during play time.
        /// </summary>
        public List<GameSession> PlayerSessions = new List<GameSession>();
        
        // Settings.
        /// <summary>
        /// Should the game music be enabled.
        /// </summary>
        public bool musicEnabled = true;

        /// <summary>
        /// Should the game sfx be enabled.
        /// </summary>
        public bool sfxEnabled = true;
    }
}