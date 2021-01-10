using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ONCGM.Utility;
using ONCGM.VR.VREnums;
using ONCGM.VR.VRInput;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ONCGM.Game {
    public class ColorsGameGenerator : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField] private float minimumTimeInBetweenAnimations = 0.25f;
        [SerializeField, Range(0.01f, 0.5f)] private float fadeInAnimationSpeed = 0.075f;
        [SerializeField, Range(0f, 1f)] private float backgroundMaxAlpha = 0.5f;
        [SerializeField] private GameObject hitCanvasPrefab;
        [SerializeField] private string hitCanvasPrefabPath = "Prefabs/UI/Hit or Miss Canvas";

        [Header("Audio Clips")] 
        [SerializeField] private AudioClip upClip;
        [SerializeField] private AudioClip downClip, leftClip, rightClip;
        private List<AudioClip> directionClips = new List<AudioClip>();
        
        // Components
        private TextMeshPro inputText;
        private AudioSource aSource;
        
        // Sprite lists
        private readonly List<SpriteRenderer> mainArrowSprites = new List<SpriteRenderer>();
        private readonly List<SpriteRenderer> backgroundArrowSprites = new List<SpriteRenderer>();
        private readonly List<SpriteRenderer> bottomArrowSprites = new List<SpriteRenderer>();
        private readonly List<SpriteRenderer> leftArrowSprites = new List<SpriteRenderer>();
        private readonly List<SpriteRenderer> rightArrowSprites = new List<SpriteRenderer>();
        private readonly List<SpriteRenderer> topArrowSprites = new List<SpriteRenderer>();
        
        // Input lists
        private List<SpawnDirection> generatedInputs = new List<SpawnDirection>();
        private List<SpawnDirection> playerInputs = new List<SpawnDirection>();
        
        /// <summary>
        /// The inputs the player has done successfully in this round.
        /// </summary>
        public List<SpawnDirection> PlayerInputs => playerInputs;
        
        // Events.
        public Action<SpawnDirection> SuccessfulInput;
        public Action ClearedInputs;

        // Variables
        private SpriteRenderer backgroundSprite;
        private WaitForFixedUpdate waitFixedUpdate;
        private WaitForSeconds waitForSecondsAnimation;
        private WaitForSeconds waitForSecondsRounds;
        private WaitForSeconds canvasWaitForSeconds;
        private float spritesAlpha;
        private float colorFlashAnimationSpeed;
        private float timeBetweenRounds = 0f;
        private int allowedInputMistakes = 1;
        private int maxAllowedInputMistakes = 1;
        private bool waitingForRecenter = false;

        // Properties
        public bool InputGenerateMode { get; private set; } = true;

        // Constants
        private const float MinColorFlashSpeed = 0.005f;
        private const float MaxColorFlashSpeed = 0.115f;
        private const float MinTimeBetweenRounds = 0.5f;
        private const float MaxTimeBetweenRounds = 2.5f;
        private const int MinAmountOfAllowedMistakes = 1;
        private const float DelayToWaitForCanvasAnimation = 2f;
        private static readonly int Fade = Animator.StringToHash("Fade");
        private static readonly int FadeGreen = Animator.StringToHash("Fade Green");
        
        #pragma warning restore 0649

        #region Setup and Fades
        
        /// <summary>
        /// Sets up the class and gathers the needed references.
        /// </summary>
        private void Start() {
            // Get components and references.
            var childCount = transform.childCount;

            aSource = GetComponent<AudioSource>();
            
            // Has to be in the same order as the SpawnDirection enum.
            directionClips = new List<AudioClip>{upClip, downClip, leftClip, rightClip};
            
            inputText = transform.GetChild(childCount - 1).GetComponent<TextMeshPro>();
            
            backgroundSprite = transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
            
            for(var i = 1; i < childCount - 1; i++) {
                mainArrowSprites.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
                mainArrowSprites.Add(transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>());
                
                backgroundArrowSprites.Add(transform.GetChild(i).GetChild(1).GetComponent<SpriteRenderer>());
                backgroundArrowSprites.Add(transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>());
            }
            
            // Set all sprites to be transparent.  
            foreach(var sprite in mainArrowSprites) {
                var color = sprite.color;
                color = new Color(color.r, color.g, color.b, 0f);
                sprite.color = color;
            }
            
            foreach(var sprite in backgroundArrowSprites) {
                var color = sprite.color;
                color = new Color(color.r, color.g, color.b, 0f);
                sprite.color = color;
            }
            
            // Sort the sprites to the direction arrays.
            for(var i = 0; i < mainArrowSprites.Count; i++) {
                if(i < 2) {
                    bottomArrowSprites.Add(mainArrowSprites[i]);
                } else if(i < 4) {
                    leftArrowSprites.Add(mainArrowSprites[i]);
                } else if(i < 6) {
                    rightArrowSprites.Add(mainArrowSprites[i]);
                } else {
                    topArrowSprites.Add(mainArrowSprites[i]);
                }
            }
            
            // Event listeners.
            ColorsMinigameController.OnMinigameBegun.AddListener(FadeInSprites);
            ColorsMinigameController.OnMinigameEnded.AddListener(FadeOutSprites);
            DeviceAngleInput.OnInputChange.AddListener(CheckForInputs);
            DeviceAngleInput.OnCentered.AddListener(() => waitingForRecenter = false);
            
            // Null checks.
            if(hitCanvasPrefab == null) {
                hitCanvasPrefab = Resources.Load<GameObject>(hitCanvasPrefabPath);
            }

            // Difficulty variables.
            colorFlashAnimationSpeed = Mathf.Lerp(MinColorFlashSpeed, MaxColorFlashSpeed, 
                                                  ((float) GameManager.CurrentSettings.GameDifficulty / (float) GameDifficulty.Impossible));
            timeBetweenRounds = Mathf.Lerp(MaxTimeBetweenRounds, MinTimeBetweenRounds,
                                           ((float) GameManager.CurrentSettings.GameDifficulty / (float) GameDifficulty.Impossible));
            allowedInputMistakes = Mathf.RoundToInt(Mathf.Lerp((int) GameDifficulty.Impossible, MinAmountOfAllowedMistakes, 
                                                               (float) GameManager.CurrentSettings.GameDifficulty / (float) GameDifficulty.Impossible));
            maxAllowedInputMistakes = allowedInputMistakes;

            // 'Wait for' initialization.
            waitFixedUpdate = new WaitForFixedUpdate();
            waitForSecondsAnimation = new WaitForSeconds(minimumTimeInBetweenAnimations);
            waitForSecondsRounds = new WaitForSeconds(timeBetweenRounds);
            canvasWaitForSeconds = new WaitForSeconds(DelayToWaitForCanvasAnimation);
        }

        /// <summary>
        /// Starts a coroutine that fades in the color wheel sprites.
        /// </summary>
        private void FadeInSprites() {
            StartCoroutine(nameof(FadeIn));
        }
        
        /// <summary>
        /// Starts a coroutine that fades out the color wheel sprites.
        /// </summary>
        private void FadeOutSprites() {
            StopAllCoroutines();
            StartCoroutine(nameof(FadeOut));
        }

        /// <summary>
        /// Fades in the sprites used in the color wheel.
        /// </summary>
        private IEnumerator FadeIn() {
            yield return canvasWaitForSeconds;
            
            while(spritesAlpha < backgroundMaxAlpha) {
                spritesAlpha += fadeInAnimationSpeed;

                inputText.alpha = spritesAlpha;

                var color = backgroundSprite.color;
                color = new Color(color.r, color.g, color.b, spritesAlpha);
                backgroundSprite.color = color;

                if(color.a < (backgroundMaxAlpha * 0.2f)) {
                    foreach(var sprite in backgroundArrowSprites) {
                        color = sprite.color;
                        color = new Color(color.r, color.g, color.b, spritesAlpha);
                        sprite.color = color;
                    }
                }

                yield return waitFixedUpdate;
            }

            yield return waitForSecondsRounds;
            StartCoroutine(AnimateColorSequence(GenerateInputs()));
        }
        
        
        /// <summary>
        /// Fades out the sprites used in the color wheel.
        /// </summary>
        private IEnumerator FadeOut() {
            while(spritesAlpha > 0f) {
                spritesAlpha -= fadeInAnimationSpeed * 2f;
                inputText.alpha = spritesAlpha;
                
                var color = backgroundSprite.color;
                color = new Color(color.r, color.g, color.b, spritesAlpha);
                if(color.a < backgroundMaxAlpha) backgroundSprite.color = color;

                foreach(var sprite in backgroundArrowSprites) {
                    color = sprite.color;
                    color = new Color(color.r, color.g, color.b, spritesAlpha);
                    sprite.color = color;
                }
                
                yield return waitFixedUpdate;
            }
        }
        
        #endregion

        #region Pattern Generation and Input Checks
        
        /// <summary>
        /// While on not on 'InputGenerationMode' will check to see if the player inputs were correct.
        /// </summary>
        private void CheckForInputs(InputDirection direction) {
            // If generating inputs or displaying animation and if the game hasn't started, return.
            if(InputGenerateMode || !ColorsMinigameController.HasStarted || waitingForRecenter) return;
            
            // If any of the list is a null, make a new one.
            if(generatedInputs == null) generatedInputs = new List<SpawnDirection>();
            if(playerInputs == null) playerInputs = new List<SpawnDirection>();
            
            var convertedDirection = SpawnDirection.All;
            
            switch(direction) {
                case InputDirection.Forward:
                    convertedDirection = SpawnDirection.Down;
                    break;
                case InputDirection.Left:
                    convertedDirection = SpawnDirection.Left;
                    break;
                case InputDirection.Right:
                    convertedDirection = SpawnDirection.Right;
                    break;
                case InputDirection.Backward:
                    convertedDirection = SpawnDirection.Up;
                    break;
                case InputDirection.Centered:
                    return;
            }

            // Check if the player has gotten the input correctly.
            if(convertedDirection == generatedInputs[Mathf.Max(0, playerInputs.Count)]) {
                // Success.
                playerInputs.Add(convertedDirection);
                ColorsMinigameController.CurrentSession.PontuacaoDaSessao += generatedInputs.Count;
                ColorsMinigameController.CurrentSession.QuantidadeDeAcertos++;
                var go = Instantiate(hitCanvasPrefab, transform.position, transform.rotation);
                go.GetComponent<HitFeedbackUi>().PickRandomText(true);
                go.transform.localScale = Vector3.one * 4;
                SuccessfulInput?.Invoke(convertedDirection);
            } else {
                // Miss.
                ColorsMinigameController.CurrentSession.QuantidadeDeErros++;
                allowedInputMistakes--;
                GameObject go = Instantiate(hitCanvasPrefab, transform.position, transform.rotation);
                go.GetComponent<HitFeedbackUi>().PickRandomText(false);
                go.transform.localScale = Vector3.one * 5;
            }
            
            // If the player has reached the generated input count,
            // restore errors (this assumes the inputs were all correct)
            // and generate more inputs and clear the player inputs from previous round.
            if(playerInputs.Count < generatedInputs.Count) return;
            allowedInputMistakes = maxAllowedInputMistakes;
            playerInputs.Clear();
            playerInputs.TrimExcess();
            
            InputGenerateMode = true;
            
            StartCoroutine(nameof(RoundInterval));
        }

        /// <summary>
        /// Waits a set amount of time before starting a new round. Time based on game difficulty.
        /// </summary>
        private IEnumerator RoundInterval() {
            yield return waitForSecondsRounds;

            waitingForRecenter = false;
            
            ClearedInputs?.Invoke();
            
            StartCoroutine(AnimateColorSequence(GenerateInputs()));
        }
        
        /// <summary>
        /// Generates an input and adds it to the list of generated inputs.
        /// </summary>
        private IEnumerable<SpawnDirection> GenerateInputs() {
            var input = GetAllowedInputs();
            
            if(generatedInputs != null) {
                generatedInputs.Add(input);
                return generatedInputs;
            }
            
            generatedInputs = new List<SpawnDirection> {input};
            
            ColorsMinigameController.CurrentSession.PosicaoDeCadaMovimento.Add(SpawnDirectionExtension.ToString(input));
            
            return generatedInputs;
        }

        /// <summary>
        /// Returns a random input based on the current game settings.
        /// </summary>
        private SpawnDirection GetAllowedInputs() {
            switch(GameManager.CurrentSettings.DirectionsToSpawnObjects) {
                case SpawnDirection.All:
                    return (SpawnDirection) Random.Range(0, (int) SpawnDirection.UpAndDown);
                case SpawnDirection.Up:
                    return SpawnDirection.Up;
                case SpawnDirection.Left:
                    return SpawnDirection.Left;
                case SpawnDirection.Right:
                    return SpawnDirection.Right;
                case SpawnDirection.Down:
                    return SpawnDirection.Down;
                case SpawnDirection.UpAndDown:
                    return Random.value > 0.5f ? SpawnDirection.Up : SpawnDirection.Down;
                case SpawnDirection.LeftAndRight:
                    return Random.value > 0.5f ? SpawnDirection.Left : SpawnDirection.Right;
                case SpawnDirection.UpAndLeft:
                    return Random.value > 0.5f ? SpawnDirection.Up : SpawnDirection.Left;
                case SpawnDirection.UpAndRight:
                    return Random.value > 0.5f ? SpawnDirection.Up : SpawnDirection.Right;
                case SpawnDirection.DownAndLeft:
                    return Random.value > 0.5f ? SpawnDirection.Down : SpawnDirection.Left;
                case SpawnDirection.DownAndRight:
                    return Random.value > 0.5f ? SpawnDirection.Down : SpawnDirection.Right;
                default:
                    return (SpawnDirection) Random.Range(0, (int) SpawnDirection.UpAndDown);
            }
        }
        

        #endregion

        #region Animations
        
        /// <summary>
        /// Trigger a full sequence of animations based on the direction passed as a parameter.
        /// </summary>
        private IEnumerator AnimateColorSequence(IEnumerable<SpawnDirection> directions) {
            foreach(var direction in directions.Where(direction => direction == SpawnDirection.Down ||
                                                                   direction == SpawnDirection.Left ||
                                                                   direction == SpawnDirection.Right ||
                                                                   direction == SpawnDirection.Up)) {
                inputText.text = SpawnDirectionExtension.ToString(direction);
                
                if(!ColorsMinigameController.HasStarted) yield break;
                    
                yield return AnimateColorFlash(GetRenderersBasedOnDirection(direction), direction);
                yield return waitForSecondsAnimation;
            }

            InputGenerateMode = false;

            StartCoroutine(nameof(AnimatePositionProximity));
        }
        
        /// <summary>
        /// Animates how close the player in to an input position in the colors display.
        /// Should only work when not generating input.
        /// </summary>
        private IEnumerator AnimatePositionProximity() {
            var upRenderers = GetRenderersBasedOnDirection(SpawnDirection.Up);
            var downRenderers = GetRenderersBasedOnDirection(SpawnDirection.Down);
            var leftRenderers = GetRenderersBasedOnDirection(SpawnDirection.Left);
            var rightRenderers = GetRenderersBasedOnDirection(SpawnDirection.Right);

            while(!InputGenerateMode) {
                var alpha = Mathf.Lerp(0,1f, Mathf.InverseLerp(0f, 1f, DeviceAngleInput.InputDirectionVector.y));
                Color color;
                
                foreach(var sprite in upRenderers) {
                    color = sprite.color;
                    color = new Color(color.r, color.g, color.b, alpha);
                    sprite.color = color;
                }
                
                alpha = Mathf.Lerp(0,1f, Mathf.InverseLerp(-1f, 0f, DeviceAngleInput.InputDirectionVector.y));

                foreach(var sprite in downRenderers) {
                    color = sprite.color;
                    color = new Color(color.r, color.g, color.b, alpha);
                    sprite.color = color;
                }
                
                alpha = Mathf.Lerp(0,1f, Mathf.InverseLerp(0f, 1f, DeviceAngleInput.InputDirectionVector.x));

                foreach(var sprite in leftRenderers) {
                    color = sprite.color;
                    color = new Color(color.r, color.g, color.b, alpha);
                    sprite.color = color;
                }
                
                alpha = Mathf.Lerp(0,1f, Mathf.InverseLerp(-1f, 0f, DeviceAngleInput.InputDirectionVector.x));

                foreach(var sprite in rightRenderers) {
                    color = sprite.color;
                    color = new Color(color.r, color.g, color.b, alpha);
                    sprite.color = color;
                }
                
                yield return waitFixedUpdate;
            }
        }
        
        /// <summary>
        /// Animates the arrow flashing once using the sprite renderer alpha.
        /// </summary>
        private IEnumerator AnimateColorFlash(IReadOnlyCollection<SpriteRenderer> sprites, SpawnDirection direction) {
            if(!ColorsMinigameController.HasStarted) yield break;
            
            var alpha = 0f;
            Color color;
            
            while(alpha < 1f) {
                alpha += colorFlashAnimationSpeed * 2f;
                inputText.alpha = alpha;

                foreach(var sprite in sprites) {
                    color = sprite.color;
                    color = new Color(color.r, color.g, color.b, alpha);
                    sprite.color = color;
                }
                
                yield return waitFixedUpdate;
            }
            
            // Play SFX.
            aSource.PlayOneShot(directionClips[(int) direction]);

            while(alpha > 0f) {
                alpha -= colorFlashAnimationSpeed;
                inputText.alpha = alpha;

                foreach(var sprite in sprites) {
                    color = sprite.color;
                    color = new Color(color.r, color.g, color.b, alpha);
                    sprite.color = color;
                }
                
                yield return waitFixedUpdate;
            }
        }
        
        #endregion

        #region Miscellaneous

        /// <summary>
        /// Returns the correct list of sprite renderers based on the direction passed as a parameter.
        /// </summary>
        private List<SpriteRenderer> GetRenderersBasedOnDirection(SpawnDirection direction) {
            switch(direction) {
                case SpawnDirection.Down:
                    return bottomArrowSprites;
                case SpawnDirection.Left:
                    return leftArrowSprites;
                case SpawnDirection.Right:
                    return rightArrowSprites;
                case SpawnDirection.Up:
                    return topArrowSprites;
                default:
                    return null;
            }
        }

        #endregion
    }
}