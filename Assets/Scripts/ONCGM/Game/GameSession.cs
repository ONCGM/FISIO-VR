using System;
using System.Collections.Generic;
using ONCGM.VR.VREnums;

namespace ONCGM.Game {
    /// <summary>
    /// Holds information about a game session.
    /// Only class in portuguese so that the json file
    /// is easier to be read by health professionals.
    /// </summary>
    [Serializable]
    public class GameSession {
        /// <summary>
        /// The name of the player who generated this session data.
        /// </summary>
        public string Nome = "Jogador";
        
        /// <summary>
        /// The age of the player who generated this session data.
        /// </summary>
        public int Idade = 0;
        
        /// <summary>
        /// The patient medical id.
        /// </summary>
        public int IdPaciente = 0;
        
        /// <summary>
        /// The session id.
        /// </summary>
        public int IdSessao = 0;
        
        /// <summary>
        /// Did the player stand for this session.
        /// </summary>
        public bool JogouDePe = false;

        /// <summary>
        /// The minimum angle setting on the game settings class. Used by the game manager.
        /// </summary>
        public float AnguloMinimo = 0f;

        /// <summary>
        /// The minimum time to validate input setting on the game settings class. Used by the game manager.
        /// </summary>
        public float TempoMinimoParaContarAcerto = 0f;

        /// <summary>
        /// The game difficulty setting on the game settings class. Used by the game manager.
        /// </summary>
        public int Dificuldade = 0;
        
        /// <summary>
        /// What minigame was this session relevant to.
        /// </summary>
        public Minigames MinijogoDestaSessao = Minigames.All;
        
        /// <summary>
        /// The setting of the included minigames on the game settings class. Used by the game manager.
        /// </summary>
        public Minigames SelecaoDeMinijogos = Minigames.All;

        /// <summary>
        /// A list of every angle the player reached on any given input during a game session.
        /// </summary>
        public List<float> AnguloDeCadaMovimento = new List<float>();
        
        /// <summary>
        /// A list of every direction the player moved towards on any given input during a game session.
        /// </summary>
        public List<string> DirecaoDeCadaMovimento = new List<string>();
        
        /// <summary>
        /// List of the position that every object spawned.
        /// </summary>
        public List<string> PosicaoDeCadaMovimento = new List<string>();
        
        /// <summary>
        /// The amount of times the player made an input in the session.
        /// Meaning the total amount of repetitions that the player performed.
        /// </summary>
        public int QuantidadeDeAcertos = 0;
        
        /// <summary>
        /// The amount of times the player made an input in the session.
        /// Meaning the total amount of repetitions that the player performed.
        /// </summary>
        public int QuantidadeDeErros = 0;
        
        /// <summary>
        /// The time and date of the session collected when it started.
        /// </summary>
        public DateTime HoraNoInicioDaSessao = new DateTime();
        
        /// <summary>
        /// How long the session lasted in minutes.
        /// </summary>
        public float TempoTotalDaSessao = 0f;

        /// <summary>
        /// The score that the player reached in this game session.
        /// </summary>
        public float PontuacaoDaSessao = 0f;

        /// <summary>
        /// The average angle delta variation during the session.
        /// </summary>
        public float MediaDeInclinacao = 0f;
    }
}