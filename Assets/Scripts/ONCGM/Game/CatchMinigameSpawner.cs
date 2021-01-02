using System;
using System.Collections;
using System.Collections.Generic;
using ONCGM.VR.VREnums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ONCGM.Game {
    /// <summary>
    /// Controls the spawn of the flying objects.
    /// </summary>
    public class CatchMinigameSpawner : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Settings")] 
        [SerializeField, Range(2, 15)] private int minimumSpawnTime = 5;
        [SerializeField, Range(2f, 15f)] private float spawnTimeBaseValue = 10f;
        [Header("Spawners")] [SerializeField] private Transform topSpawner;
        [SerializeField] private Transform middleSpawner;
        [SerializeField] private Transform bottomSpawner;
        [SerializeField] private Transform playerHead;

        [Header("Butterfly Prefabs")] [SerializeField]
        private GameObject[] butterflyPrefabs;

        [Header("Helicopter Prefabs")] [SerializeField]
        private GameObject[] helicopterPrefabs;

        // Variables.
        private WaitForSeconds waitForSeconds;
        private float waitTime = 1f;
        private float angle = 0f;
        private SpawnObjectsCatchGame typeOfObject = SpawnObjectsCatchGame.All;
        private SpawnDirection allowedDirections = SpawnDirection.All;

        // Next lane to spawn an object.
        private SpawnDirection nextSpawnPosition = SpawnDirection.Up;

        #pragma warning restore 0649

        /// <summary>
        /// Sets up the class variables and references.
        /// </summary>
        private void Start() {
            if(topSpawner == null || middleSpawner == null || bottomSpawner == null && transform.childCount == 3) {
                topSpawner = transform.GetChild(0);
                middleSpawner = transform.GetChild(1);
                bottomSpawner = transform.GetChild(2);
            }

            CatchMinigameController.OnMinigameBegun.AddListener(StartSpawning);
            angle = GameManager.CurrentSettings.MinimumAngle;
            typeOfObject = GameManager.CurrentSettings.TypesOfObjectsToSpawn;
            allowedDirections = GameManager.CurrentSettings.DirectionsToSpawnObjects;
            SetupSpawner();
        }

        /// <summary>
        /// Triggers the spawn of objects.
        /// </summary>
        private void StartSpawning() {
            // Sets the time based on a factor of the total session time and amount of minigames.
            // Update this to do so when more minigames are added.
            waitTime = (GameManager.CurrentSettings.TotalSessionTime * 60f) /
                       (spawnTimeBaseValue + (float) GameManager.CurrentSettings.GameDifficulty);

            waitForSeconds = new WaitForSeconds(waitTime);

            StartCoroutine(nameof(SpawnObjects));
        }

        /// <summary>
        /// Spawns an object every x seconds and restarts if the game hasn't ended.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnObjects() {
            nextSpawnPosition = SelectNextSpawner();
            if(!GameManager.IsPaused) {
                if(nextSpawnPosition != SpawnDirection.All) {
                    switch(nextSpawnPosition) {
                        case SpawnDirection.Up:
                            if(CheckIfSpawnIsAvailable(nextSpawnPosition)) 
                                Instantiate(typeOfObject == SpawnObjectsCatchGame.All ? (Random.value > 0.5f ? helicopterPrefabs[0] : butterflyPrefabs[0])
                                                : typeOfObject == SpawnObjectsCatchGame.Helicopter ? helicopterPrefabs[0] : butterflyPrefabs[0], topSpawner);
                            break;
                        case SpawnDirection.Left:
                            if(CheckIfSpawnIsAvailable(nextSpawnPosition)) 
                                Instantiate(typeOfObject == SpawnObjectsCatchGame.All ? (Random.value > 0.5f ? helicopterPrefabs[1] : butterflyPrefabs[1])
                                            : typeOfObject == SpawnObjectsCatchGame.Helicopter ? helicopterPrefabs[1] : butterflyPrefabs[1], middleSpawner);
                            break;
                        case SpawnDirection.Right:
                            if(CheckIfSpawnIsAvailable(nextSpawnPosition)) 
                                Instantiate(typeOfObject == SpawnObjectsCatchGame.All ? (Random.value > 0.5f ? helicopterPrefabs[2] : butterflyPrefabs[2])
                                            : typeOfObject == SpawnObjectsCatchGame.Helicopter ? helicopterPrefabs[2] : butterflyPrefabs[2], middleSpawner);
                            break;
                        case SpawnDirection.Down:
                            if(CheckIfSpawnIsAvailable(nextSpawnPosition))
                                Instantiate(typeOfObject == SpawnObjectsCatchGame.All ? (Random.value > 0.5f ? helicopterPrefabs[3] : butterflyPrefabs[3])
                                            : typeOfObject == SpawnObjectsCatchGame.Helicopter ? helicopterPrefabs[3] : butterflyPrefabs[3], bottomSpawner);
                            break;
                    }

                    if(CheckIfSpawnIsAvailable(nextSpawnPosition)) CatchMinigameController.CurrentSession.PosicaoDeCadaMovimento.Add(
                        SpawnDirectionCatchGameExtension.ToString(nextSpawnPosition));
                } else {
                    StopCoroutine(nameof(SpawnObjects));
                    StartCoroutine(nameof(SpawnObjects));
                    yield break;
                }
            }

            yield return waitForSeconds;
            waitForSeconds = new WaitForSeconds(waitTime > minimumSpawnTime
                                                    ? waitTime -=
                                                        waitTime * (1f + (float) GameManager
                                                                                 .CurrentSettings.GameDifficulty * 0.1f)
                                                    : minimumSpawnTime);
            if(CatchMinigameController.HasStarted) {
                StartCoroutine(nameof(SpawnObjects));
            }
        }
        
        /// <summary>
        /// Checks if the selected spawn has a object in it.
        /// </summary>
        /// <param name="spawnDirection"> The spawn to check for. </param>
        /// <returns> Returns true if the spawn is available. </returns>
        private bool CheckIfSpawnIsAvailable(SpawnDirection spawnDirection) {
            switch(spawnDirection) {
                case SpawnDirection.Up:
                    return topSpawner.childCount < 1;
                case SpawnDirection.Down:
                    return bottomSpawner.childCount < 1;
                case SpawnDirection.Left:
                    return middleSpawner.childCount < 1;
                case SpawnDirection.Right:
                    return middleSpawner.childCount < 1;
                case SpawnDirection.All:
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Selects a spawner based on the amount of object the game has already spawned.
        /// </summary>
        private SpawnDirection SelectNextSpawner() {
            switch(allowedDirections) {
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
            }

            return SpawnDirection.All;
        }


        /// <summary>
        /// Sets up the spawner position based on game settings.
        /// </summary>
        private void SetupSpawner() {
            topSpawner.transform.localPosition = new Vector3(0f, GetSpawnHeightBasedOnAngle(), 0f);
            bottomSpawner.transform.localPosition = new Vector3(0f, -GetSpawnHeightBasedOnAngle(), 0f);
        }
        
        /// <summary>
        /// Calculates the height of the spawn based on angle settings.
        /// </summary>
        private float GetSpawnHeightBasedOnAngle() {
            return Vector3.Distance(playerHead.position, transform.position) * Mathf.Tan(angle * Mathf.Deg2Rad);
        }
    }
}