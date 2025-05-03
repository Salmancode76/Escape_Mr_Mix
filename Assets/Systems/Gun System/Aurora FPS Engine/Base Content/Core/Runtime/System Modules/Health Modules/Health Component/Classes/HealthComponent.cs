/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    public abstract class HealthComponent : MonoBehaviour, IHealth, IDamageable
    {
        #region [IHealth Implementation]
        /// <summary>
        /// Get current health point.
        /// </summary>
        public abstract float GetHealth();

        /// <summary>
        /// Take damage to the health.
        /// </summary>
        /// <param name="amount">Damage amount.</param>
        public abstract void TakeDamage(float amount);

        /// <summary>
        /// Alive state of health.
        /// </summary>
        /// <returns>
        /// True if health > 0.
        /// Otherwise false.
        /// </returns>
        public abstract bool IsAlive();
        #endregion
    }
}