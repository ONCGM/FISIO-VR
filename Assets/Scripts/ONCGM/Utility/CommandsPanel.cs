using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Updates the commands list with the last 6 successful player inputs.
    /// </summary>
    public class CommandsPanel : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField] private GameObject commandEntryPrefab;
        [SerializeField] private Transform commandEntriesParent;
        [SerializeField, Range(0.1f, 3f)] private float fadeAnimationTime = 1f;
        [SerializeField, Range(1, 6)] private int commandsLimit = 6;
        
        // References and Components.
        private CanvasGroup canvasGroup;
        private ColorsGameGenerator generator;
        
        #pragma warning restore 0649
        
        // Setup.
        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            generator = FindObjectOfType<ColorsGameGenerator>();
            generator.SuccessfulInput += UpdateCommandList;
            generator.ClearedInputs += ClearCommandsList;
            
            // Animation.
            canvasGroup.alpha = 0f;
            ColorsMinigameController.OnMinigameEnded.AddListener(() => {
                DOTween.To(()=> canvasGroup.alpha, x=> canvasGroup.alpha = x, 0f, fadeAnimationTime);
            });
            
            ColorsMinigameController.OnMinigameBegun.AddListener(() => {
                DOTween.To(()=> canvasGroup.alpha, x=> canvasGroup.alpha = x, 1f, fadeAnimationTime);
            });
        }

        // Removes action reference.
        private void OnDestroy() {
            // ReSharper disable once DelegateSubtraction
            generator.SuccessfulInput -= UpdateCommandList;
            // ReSharper disable once DelegateSubtraction
            generator.ClearedInputs -= ClearCommandsList;
        }

        /// <summary>
        /// Updates the command list to match the player inputs.
        /// </summary>
        private void UpdateCommandList(SpawnDirection direction) {
            var command = Instantiate(commandEntryPrefab, commandEntriesParent).GetComponent<CommandListEntry>();
            command.SetDirection(direction);

            if(commandEntriesParent.childCount > commandsLimit) {
                Destroy(commandEntriesParent.GetChild(0).gameObject);
            }
        }


        /// <summary>
        /// Clears the commands list for when resetting a round. 
        /// </summary>
        private void ClearCommandsList(){
            for(var i = 0; i < commandEntriesParent.childCount; i++) {
                Destroy(commandEntriesParent.GetChild(i).gameObject);
            }

            foreach(var entry in FindObjectsOfType<CommandListEntry>()) {
                Destroy(entry.gameObject);
            }
        }
    }
}