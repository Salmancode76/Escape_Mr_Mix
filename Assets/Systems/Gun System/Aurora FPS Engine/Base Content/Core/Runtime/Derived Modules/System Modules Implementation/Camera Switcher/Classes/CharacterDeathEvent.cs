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
    public abstract class CharacterDeathEvent
    {
        /// <summary>
        /// Initialize death camera system instance.
        /// Called inside Awake() body method of health system component
        /// </summary>
        public virtual void Initialize(HealthComponent healthComponent) { }

        /// <summary>
        /// Called once when object health become zero.
        /// </summary>
        public abstract void OnDead();
    }
}

