using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ONCGM.VR.VRButtons {
    /// <summary>
    /// Opens the specified URL whenever the 'OpenUrlRequest' method is called.
    /// </summary>
    public class OpenUrl : MonoBehaviour {
        private const string FeevaleUrl = "https://www.feevale.br/";

        /// <summary>
        /// Tries to open the specified url through Unity's 'Application' class.
        /// </summary>
        public static void OpenUrlRequest(string url = "https://www.feevale.br/") {
            Application.quitting += () =>  Application.OpenURL(url);
        }
        
        /// <summary>
        /// Tries to open the specified url through Unity's 'Application' class.
        /// </summary>
        public void OpenFeevaleWebPageRequest() {
            Application.quitting += () =>  Application.OpenURL(FeevaleUrl);
        }
    }
}