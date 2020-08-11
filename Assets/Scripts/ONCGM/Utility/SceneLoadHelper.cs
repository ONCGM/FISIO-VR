using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Used to yield load activation and prevent screen flash on load.
    /// </summary>
    public class SceneLoadHelper : MonoBehaviour {
        /// <summary>
        /// Sets up the object to not destroy on load, or if there's already one spawned, self destructs.
        /// </summary>
        private void Awake() {
            DontDestroyOnLoad(this);
            if(GameObject.FindObjectsOfType<SceneLoadHelper>().Length > 1) {
                Destroy(gameObject);
            } else {
                Destroy(GetComponent<MeshRenderer>());
                Destroy(GetComponent<MeshFilter>());
                Destroy(GetComponent<MeshCollider>());
            }
        }

        /// <summary>
        /// Starts the coroutine for waiting the load time.
        /// </summary>
        public void WaitForSceneToLoad(AsyncOperation async) {
            StartCoroutine(WaitForLoadFinish(async));
        }
        
        /// <summary>
        /// Waits for a scene to be completely loaded and the allows the next one to activate.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForLoadFinish(AsyncOperation async) {
            async.allowSceneActivation = false;
            
            while(async.progress < 0.9f) {
                yield return null;
            }

            async.allowSceneActivation = true;
        }
    }
}