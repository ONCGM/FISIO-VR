using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ONCGM.Utility {
    /// <summary>
    /// Used by animation to emit or trigger particles on demand.
    /// </summary>
    public class EmitParticlesByAnimation : MonoBehaviour {
        #pragma warning disable 0649
        [Header("Objects References (If left blank will search on self.)")]
        [SerializeField] private GameObject gameObjectToSearchForParticleSystems;
        [Header("Settings")]
        [SerializeField] private int particleSystemBurstIndexToSample = 0;
        [SerializeField] private bool overrideParticleSystemBurstAmount = false;
        [SerializeField] private int amountOfParticlesOnEmit = 10;

        // Variables.
        private List<ParticleSystem> particleSystems;
        
        #pragma warning restore 0649
        /// <summary>
        /// Gets the emitters attached to this object as reference in a variable. A.K.A Sets up the class.
        /// </summary>
        private void Awake() {
            particleSystems = gameObjectToSearchForParticleSystems != null ? gameObjectToSearchForParticleSystems.GetComponents<ParticleSystem>().ToList() : GetComponents<ParticleSystem>().ToList();
        }

        /// <summary>
        /// Emits the particle(s) attached to this script.
        /// </summary>
        public void EmitParticles() {
            foreach(var system in particleSystems) {
                system.Emit(overrideParticleSystemBurstAmount ? amountOfParticlesOnEmit : (int) system.emission.GetBurst(particleSystemBurstIndexToSample).count.constant);
            }
        }

        /// <summary>
        /// Restarts the particle(s) attached to this script.
        /// </summary>
        public void RestartParticles() {
            foreach(var system in particleSystems) {
                system.Stop();
                system.Play();
            }
        }
        
        /// <summary>
        /// Plays the particle(s) attached to this script.
        /// </summary>
        public void PlayParticles() {
            foreach(var system in particleSystems) {
                system.Play();
            }
        }

        /// <summary>
        /// Stops the emitting particle(s) attached to this script.
        /// </summary>
        public void StopParticles() {
            foreach(var system in particleSystems) {
                system.Stop();
            }
        } 
    }
}