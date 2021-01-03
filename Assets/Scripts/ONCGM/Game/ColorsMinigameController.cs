using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ONCGM.Utility;
using ONCGM.VR.VREnums;
using UnityEngine;
using UnityEngine.Events;

namespace ONCGM.Game {
    public class ColorsMinigameController : MonoBehaviour {
        #pragma warning disable 0649
        
        // Variables.
        
        /// <summary>
        /// The GameSession for this minigame.
        /// </summary>
        public static GameSession CurrentSession { get; set; } = new GameSession();
        /// <summary>
        /// How many seconds are left in the current session.
        /// </summary>
        public static int SecondsLeftInMinigame { get; private set; } = 120;
        /// <summary>
        /// Has the minigame started.
        /// </summary>
        public static bool HasStarted { get; private set; } = true;
        /// <summary>
        /// Used in the minigame timer.
        /// </summary>
        private WaitForSecondsRealtime waitASecond;

        // Events.
        /// <summary>
        /// Called when the minigame starts.
        /// </summary>
        public static UnityEvent OnMinigameBegun = new UnityEvent();
        /// <summary>
        /// Called when the minigame has ended.
        /// </summary>
        public static UnityEvent OnMinigameEnded = new UnityEvent();
        /// <summary>
        /// Called every in-game second.
        /// </summary>
        public static UnityEvent Tick = new UnityEvent();
        #pragma warning restore 0649

        #region Unity Events and Functions
        /// <summary>
        /// Sets up the class and the public events up.
        /// </summary>
        private void Awake() {
            waitASecond = new WaitForSecondsRealtime(1);
            if(GameObject.FindObjectsOfType<ColorsMinigameController>().Length > 1) Destroy(this);
        }

        #endregion
        
        #region Minigame Control
        /// <summary>
        /// Starts the minigame and notifies all classes who subscribed to the OnMinigameBegun event.
        /// </summary>
        public void BeginMinigame() {
            Debug.Log($"Minigame {GameManager.CurrentMinigame.ToString()} Started.");

            if(GameManager.CurrentSettings.MinigamesToIncludeInSession == Minigames.ColorsGame) {
                SecondsLeftInMinigame = Mathf.CeilToInt(GameManager.CurrentSettings.TotalSessionTime * 60f);
            } else if(GameManager.CurrentSettings.MinigamesToIncludeInSession == Minigames.All) {
                SecondsLeftInMinigame = Mathf.CeilToInt((GameManager.CurrentSettings.TotalSessionTime * 60f) / 2f);
            } else if(GameManager.CurrentSettings.MinigamesToIncludeInSession == Minigames.CatchAndColors ||
                      GameManager.CurrentSettings.MinigamesToIncludeInSession == Minigames.ColorsAndFlying) {
                SecondsLeftInMinigame = Mathf.CeilToInt((GameManager.CurrentSettings.TotalSessionTime * 60f) / 2f);
            } 

            HasStarted = true;
            
            CurrentSession = new GameSession {
                HoraNoInicioDaSessao = DateTime.Now, AnguloDeCadaMovimento = new List<float>(),
                DirecaoDeCadaMovimento = new List<string>()
            };
            
            StartCoroutine(nameof(MinigameTimer));
            OnMinigameBegun.Invoke();
        }
        
        /// <summary>
        /// Counts down the minigame timer until it reaches zero.
        /// </summary>
        private IEnumerator MinigameTimer() {
            yield return waitASecond;
            if(!GameManager.IsPaused) {
                Tick.Invoke();
                if(SecondsLeftInMinigame > 0) {
                    SecondsLeftInMinigame -= 1;
                    StartCoroutine(nameof(MinigameTimer));
                } else {
                    EndMinigame();
                }
            } else {
                StartCoroutine(nameof(MinigameTimer));
            }
        }
        
        /// <summary>
        /// Updates the CurrentSession property, adds it to the save file sessions and saves it back to the file.
        /// </summary>
        public static void SaveSessionData() {
            CurrentSession.Nome = SaveSystem.LoadedData.PlayerName;
            CurrentSession.Idade = SaveSystem.LoadedData.PlayerAge;
            CurrentSession.JogouDePe = SaveSystem.LoadedData.isStanding;
            CurrentSession.IdPaciente = SaveSystem.LoadedData.PatientId;
            CurrentSession.IdSessao = SaveSystem.LoadedData.TotalSessions;
            CurrentSession.TempoTotalDaSessao = GameManager.CurrentSettings.TotalSessionTime;
            CurrentSession.AnguloDeCadaMovimento.TrimExcess();
            CurrentSession.DirecaoDeCadaMovimento.TrimExcess();
            CurrentSession.PosicaoDeCadaMovimento.TrimExcess();
            if(CurrentSession.AnguloDeCadaMovimento.Count > 2) {
                CurrentSession.MediaDeInclinacao = CurrentSession.AnguloDeCadaMovimento.Average(x => x);
            } else if(CurrentSession.AnguloDeCadaMovimento.Count == 1){
                CurrentSession.MediaDeInclinacao = CurrentSession.AnguloDeCadaMovimento[0];
            }
            CurrentSession.Dificuldade = (int) GameManager.CurrentSettings.GameDifficulty;
            CurrentSession.AnguloMinimo = GameManager.CurrentSettings.MinimumAngle;
            CurrentSession.MinijogoDestaSessao = GameManager.CurrentMinigame;
            CurrentSession.SelecaoDeMinijogos = GameManager.CurrentSettings.MinigamesToIncludeInSession;
            CurrentSession.TempoMinimoParaContarAcerto = GameManager.CurrentSettings.MinimumTimeToValidateInput;
            CurrentSession.DeteccaoSuavizada = GameManager.CurrentSettings.useSmoothedInput;

            SaveSystem.LoadedData.SessoesDeJogo.Add(CurrentSession);
            SaveSystem.SaveGameToFile();
            SaveSystem.ExportSessionDataAsJson(CurrentSession);
            SaveSystem.ExportAllSessionsDataAsJson();
        }
        
        /// <summary>
        /// Ends the minigame and notifies all classes who subscribed to the OnMinigameEnded event.
        /// </summary>
        public static void EndMinigame() {
            Debug.Log("Minigame finished.");
            HasStarted = false;
            OnMinigameEnded.Invoke();
            SaveSystem.LoadedData.TotalSessions++;
            SaveSessionData();
        }
        #endregion

    }
}
