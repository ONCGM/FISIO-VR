using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ONCGM.Game;
using ONCGM.VR.VREnums;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Class to save and load files.
    /// </summary>
    public static class SaveSystem {        

        public static SaveData LoadedData { get; set; }
        private static string DirectoryName { get; set; } = "Dados";
        
        private static string SessionDirectoryName { get; set; } = "Sessoes";
        private static string SaveFileName { get; set; } = "data.vr";
        private static string JsonFileName { get; set; } = "sessão";
        
        private static string CompleteJsonFileName { get; set; } = "sessões_fisio_vr";
        private static string SaveFilePath { get; set; }
        private static string JsonFilePath { get; set; }
        
        private static readonly DirectoryInfo Directory;
        private static readonly DirectoryInfo SessionsDirectory;

        static SaveSystem() {
            SaveFilePath = Path.Combine(Application.persistentDataPath, DirectoryName, SaveFileName);
            JsonFilePath = Path.Combine(Application.persistentDataPath, DirectoryName, string.Concat(JsonFileName, GetCurrentDataFormatted(),".json"));
            Directory = new DirectoryInfo(Path.Combine(Application.persistentDataPath, DirectoryName));
            SessionsDirectory = new DirectoryInfo(Path.Combine(Application.persistentDataPath, DirectoryName, SessionDirectoryName));
            CheckForDirectory();
            LoadedData = LoadGameFile();
        }

        /// <summary>
        /// Checks if the game folder exists, if it doesn't, it will create it.
        /// </summary>
        private static void CheckForDirectory() {
            if(Directory.Exists && SessionsDirectory.Exists) {
                return;
            }

            if(!Directory.Exists) Directory.Create();
            if(!SessionsDirectory.Exists) SessionsDirectory.Create();
        }

        /// <summary>
        /// Checks if a save file exists.
        /// </summary>
        public static bool CheckForSaveFile() {
            return File.Exists(SaveFilePath);
        }

        /// <summary>
        /// Save the specified data, if it receives a null it will not save anything.
        /// </summary>
        public static void SaveGameToFile() {
            CheckForDirectory();
            
            if(LoadedData == null) {
                LoadedData = new SaveData();
            }
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(SaveFilePath, FileMode.Create);
            bf.Serialize(fs, LoadedData);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Close();
        }

        /// <summary>
        /// Checks if there is a file to load, if it does not find one, it wil create a new one.
        /// <para>
        /// If it can't create a new one, it will return a null.
        /// </para>
        /// </summary>
        public static SaveData LoadGameFile() {        
            CheckForDirectory();
            if(CheckForSaveFile()) {
                try { 
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fs = new FileStream(SaveFilePath, FileMode.Open);
                    SaveData data = bf.Deserialize(fs) as SaveData;
                    fs.Close();
                    return data;
                } catch(Exception e) {
                    Console.WriteLine(e);
                    LoadedData = new SaveData();
                    SaveGameToFile();
                    return new SaveData();
                }
            } else {
                SaveGameToFile();
                if(CheckForSaveFile()) {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fs = new FileStream(SaveFilePath, FileMode.Open);
                    SaveData data = bf.Deserialize(fs) as SaveData;
                    fs.Close();
                    return data;
                } else {
                    LoadedData = new SaveData();
                    SaveGameToFile();
                    return new SaveData();
                }
            }
        }
        
        /// <summary>
        /// Converts the save file data to json and saves it to the same directory as the save file.
        /// </summary>
        public static void ExportSessionDataAsJson([NotNull] GameSession session) {
            JsonFilePath = Path.Combine(Application.persistentDataPath, DirectoryName, SessionDirectoryName, 
                                        $"{JsonFileName}-{session.IdSessao}_nome-{session.Nome}_id-{session.IdPaciente}_{GetCurrentDataFormatted()}.json");
            string jsonData = JsonConvert.SerializeObject(session, Formatting.Indented);
            File.WriteAllText(JsonFilePath, jsonData, Encoding.ASCII);
            UiAudioHandler.PlayClip(UiAudioClips.SaveSuccessful);
        }
        
        /// <summary>
        /// Converts the save file data to json and saves it to the same directory as the save file.
        /// </summary>
        public static void ExportAllSessionsDataAsJson() {
            JsonFilePath = Path.Combine(Application.persistentDataPath, DirectoryName, $"{CompleteJsonFileName}.json");
            string jsonData = JsonConvert.SerializeObject(LoadedData.SessoesDeJogo, Formatting.Indented);
            File.WriteAllText(JsonFilePath, jsonData, Encoding.ASCII);
            UiAudioHandler.PlayClip(UiAudioClips.SaveSuccessful);
        }

        /// <summary>
        /// Gets the current time and formats it into a savable format.
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentDataFormatted() {
            return $"d{DateTime.Now.Day}_m{DateTime.Now.Month}_a{DateTime.Now.Year}_as_{DateTime.Now.Hour}h{DateTime.Now.Minute}m{DateTime.Now.Second}s";
        }
    }
}