using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ONCGM.Utility;
using ONCGM.VR.VREnums;
using ONCGM.VR.VRInput;
using UnityEngine;

namespace ONCGM.Game {
    /// <summary>
    /// An object that can be catched and scores point in the catch minigame.
    /// </summary>
    public class CatchableObject : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField] private string playerTag = "Player";
        [SerializeField, Range(1, 50)] private int scoreAmount = 1; 
        [SerializeField] private Color colorToFadeInTimeout = Color.red;
        [SerializeField] private InputDirection directionToCheckInput = InputDirection.InvalidDirection;
        [SerializeField] private GameObject hitCanvasPrefab;
        [SerializeField] private string hitCanvasPrefabPath = "Prefabs/UI/Hit or Miss Canvas";
        [SerializeField] private GameObject[] triangleArrows;
        
        // Variables.
        private Animator anim;
        private Animator arrowAnim;
        private const float AmountOfTimeToAccountForDifficulty = 10f;
        private float colorTransitionAmount = 0f;
        private WaitForEndOfFrame waitForFrame;
        private WaitForSeconds waitForSeconds;
        #pragma warning disable 414
        private bool isColliding;
        #pragma warning restore 414
        private bool checkInput;
        private static readonly int DirectionY = Animator.StringToHash("DirectionY");
        private static readonly int DirectionX = Animator.StringToHash("DirectionX");
        private static readonly int Exit = Animator.StringToHash("Exit");
        private readonly Color initialMaterialColor = Color.white;
        private SpriteRenderer[] spriteRenderers;
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up the class and gets needed references.
        /// </summary>
        private void Start() {
            anim = GetComponent<Animator>();
            arrowAnim = transform.GetChild(1).GetComponent<Animator>();
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            
            foreach(var triangle in triangleArrows) {
                triangle.SetActive(false);
            }
            
            switch(directionToCheckInput) {
                case InputDirection.Forward:
                    anim.SetFloat(DirectionX, 0f);
                    anim.SetFloat(DirectionY, 1f);
                    break;
                case InputDirection.Left:
                    anim.SetFloat(DirectionX, -1f);
                    anim.SetFloat(DirectionY, 0f);
                    break;
                case InputDirection.Right:
                    anim.SetFloat(DirectionX, 1f);
                    anim.SetFloat(DirectionY, 0f);
                    break;
                case InputDirection.Backward:
                    anim.SetFloat(DirectionX, 0f);
                    anim.SetFloat(DirectionY, -1f);
                    break;
            }
            
            if(hitCanvasPrefab == null) {
                hitCanvasPrefab = Resources.Load<GameObject>(hitCanvasPrefabPath);
            }
            
            waitForFrame = new WaitForEndOfFrame();
            waitForSeconds = new WaitForSeconds((GameManager.CurrentSettings.MinimumTimeToValidateInput + AmountOfTimeToAccountForDifficulty) - (int) GameManager.CurrentSettings.GameDifficulty);
            DeviceAngleInput.OnInputChange.AddListener(CheckInput);
            
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.OnMinigameEnded.AddListener(RemoveOnEndGame);
                    CatchMinigameController.CurrentSession.PosicaoDeCadaMovimento.Add(InputDirectionExtension.ToString(directionToCheckInput));
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.OnMinigameEnded.AddListener(RemoveOnEndGame);
                    ColorsMinigameController.CurrentSession.PosicaoDeCadaMovimento.Add(InputDirectionExtension.ToString(directionToCheckInput));
                    break;
                case Minigames.FlyingGame:
                    // Add later
                    break;
            }
        }

        /// <summary>
        /// Starts the time out timer. Intended to be used by an animator.
        /// </summary>
        public void StartTimer() {
            checkInput = true;
            StartCoroutine(nameof(TimeOut));
            DOVirtual.Float(0f, 1f, ((GameManager.CurrentSettings.MinimumTimeToValidateInput + AmountOfTimeToAccountForDifficulty) - (int) GameManager.CurrentSettings.GameDifficulty), 
                            value => colorTransitionAmount = value);
            StartCoroutine(nameof(LerpMaterialColor));
            
            foreach(var triangle in triangleArrows) {
                triangle.SetActive(true);
            }

            GetComponentInChildren<CatchGameProximityArrows>().DisplayProximity();
        }

        /// <summary>
        /// Triggers the exit animation for the object based on its current direction.
        /// </summary>
        private void ExitSceneAnimation() {
            anim.SetTrigger(Exit);
            arrowAnim.SetTrigger(Exit);
            foreach(var triangle in triangleArrows) triangle.SetActive(false);
            GetComponentInChildren<CatchGameProximityArrows>().StopDisplaying();
        }
        
        /// <summary>
        /// Destroys object when the game ends or on demand.
        /// </summary>
        public void RemoveOnEndGame() {
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.OnMinigameEnded.RemoveListener(RemoveOnEndGame);
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.OnMinigameEnded.RemoveListener(RemoveOnEndGame);
                    break;
                case Minigames.FlyingGame:
                    // Add later
                    break;
            }
            
            Destroy(gameObject);
        }

        /// <summary>
        /// Checks if the current direction is met.
        /// </summary>
        private void CheckInput(InputDirection direction) {
            if(!checkInput) return;
            
            #if UNITY_EDITOR
                if(direction != directionToCheckInput) return;
            #else
                if(direction != directionToCheckInput || !isColliding) return;
            #endif
            
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.CurrentSession.QuantidadeDeAcertos++;
                    CatchMinigameController.CurrentSession.PontuacaoDaSessao +=
                        (scoreAmount * (1 + (int) GameManager.CurrentSettings.GameDifficulty));
                    StopAllCoroutines();
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.CurrentSession.QuantidadeDeAcertos++;
                    ColorsMinigameController.CurrentSession.PontuacaoDaSessao +=
                        (scoreAmount * (1 + (int) GameManager.CurrentSettings.GameDifficulty));
                    StopAllCoroutines();
                    break;
                case Minigames.FlyingGame:
                    // Add later
                    break;
            }
            
            UiAudioHandler.PlayClip(UiAudioClips.Success);
            Instantiate(hitCanvasPrefab, transform.position, transform.rotation).GetComponent<HitFeedbackUi>().PickRandomText(true);
            Destroy(gameObject);
        }

        /// <summary>
        /// Waits 'x' amount of time and if no input was detected until then, counts as a miss and destroys the object.
        /// </summary>
        /// <returns></returns>
        private IEnumerator TimeOut() {
            yield return waitForSeconds;
            switch(GameManager.CurrentMinigame) {
                case Minigames.CatchGame:
                    CatchMinigameController.CurrentSession.QuantidadeDeErros++;
                    break;
                case Minigames.ColorsGame:
                    ColorsMinigameController.CurrentSession.QuantidadeDeErros++;
                    break;
                case Minigames.FlyingGame:
                    // Add later
                    break;
            }
            
            UiAudioHandler.PlayClip(UiAudioClips.Miss);
            Instantiate(hitCanvasPrefab, transform.position, transform.rotation).GetComponent<HitFeedbackUi>().PickRandomText(false);
            ExitSceneAnimation();
        }
        
        /// <summary>
        /// Lerps the material color using DoTween.
        /// </summary>
        private IEnumerator LerpMaterialColor() {
            while(colorTransitionAmount < 1f) {
                var lerpColor = Color.Lerp(initialMaterialColor, colorToFadeInTimeout, colorTransitionAmount);
                
                foreach(var spriteRenderer in spriteRenderers) {
                    spriteRenderer.color = lerpColor;
                }
                
                yield return waitForFrame;
            }
        }
        
        #region Collision detection
        private void OnTriggerEnter(Collider other) {
            if(other.CompareTag(playerTag)) {
                isColliding = true;
            }
        }
        
        private void OnTriggerExit(Collider other) {
            if(other.CompareTag(playerTag)) {
                isColliding = false;
            }
        }
        
        #endregion
    }
}