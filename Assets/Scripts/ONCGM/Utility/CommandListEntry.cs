using System;
using System.Collections;
using System.Collections.Generic;
using ONCGM.VR.VREnums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ONCGM.Utility {
    /// <summary>
    /// Populates the info in a entry for the command list.
    /// </summary>
    public class CommandListEntry : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField] private Sprite topDirectionSprite;
        [SerializeField] private Sprite bottomDirectionSprite, 
                                        leftDirectionSprite, 
                                        rightDirectionSprite;
        private List<Sprite> directionSprites = new List<Sprite>();
        
        // Components.
        private Image directionImage;
        private TMP_Text directionText;
        
        #pragma warning restore 0649
        
        // Setup.
        private void Awake() {
            directionImage = GetComponentInChildren<Image>();
            directionText = GetComponentInChildren<TMP_Text>();
            
            // Has to be in the same order as the SpawnDirection Enum.
            directionSprites = new List<Sprite>{topDirectionSprite, bottomDirectionSprite, leftDirectionSprite, rightDirectionSprite};
        }

        /// <summary>
        /// Sets the UI values based on the specified direction.
        /// </summary>
        public void SetDirection(SpawnDirection direction) {
            directionText.text = SpawnDirectionExtension.ToString(direction);
            directionImage.sprite = directionSprites[(int) direction];
        }
    }
}