using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace ONCGM.Utility {
    public class NameAgeFieldSerializer : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Components")] 
        [SerializeField] private TMP_Text nameField;
        [SerializeField] private TMP_Text ageField;
        
        #pragma warning restore 0649
        
        /// <summary>
        /// Sets up the class and loads variables into the texts.
        /// </summary>
        private void Awake() {
            nameField.text = SaveSystem.LoadedData.PlayerName;
            ageField.text = SaveSystem.LoadedData.PlayerAge.ToString();
        }

        /// <summary>
        /// Sets the user name and age on the save file.
        /// </summary>
        public void SaveNameAndAge() {
            SaveSystem.LoadedData.PlayerName = nameField.text;
            SaveSystem.LoadedData.PlayerAge = int.Parse(ageField.text);
            SaveSystem.SaveGameToFile();
        }
    }
}

