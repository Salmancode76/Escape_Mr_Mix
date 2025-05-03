/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [System.Serializable]
    public abstract class HealthEffect
    {
        /// <summary>
        /// Implement this method to make some initialization 
        /// and get access to CharacterHealth references.
        /// </summary>
        /// <param name="healthComponent">Player health component reference.</param>
        public abstract void Initialization(CharacterHealth characterHealth);
    }
}