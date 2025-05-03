/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEngine;
using UnityEngine.Events;


namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Health/Object Health")]
    [DisallowMultipleComponent]
    public class ObjectHealth : HealthComponent
    {
        #region [Inspector Event Callback Implementation]
        [Serializable]
        public class OnTakeDamageEvent : UnityEvent<int> { }

        [Serializable]
        public class OnWakeUpTimerEvent : UnityEvent<float> { }
        #endregion

        // Base object health properties.
        [SerializeField]
        [Slider("minHealth", "maxHealth")]
        private float health = 100;

        [SerializeField]
        [MinValue(0)]
        private float minHealth = 0;

        [SerializeField]
        private float maxHealth = 100;

        /// <summary>
        /// Called once when object health become zero.
        /// Implement this method to make custom death logic.
        /// </summary>
        protected virtual void OnDead()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Called when object health become more then zero.
        /// Implement this method to make revive logic.
        /// </summary>
        protected virtual void OnRevive()
        {

        }

        /// <summary>
        /// Apply new health points.
        /// </summary>
        /// <param name="amount">Health amount.</param>
        public virtual void ApplyHealth(float amount)
        {
            SetHealth(health + Mathf.Abs(amount));
        }

        #region [Private Internal Methods]
        /// <summary>
        /// Internal OnDead method to call addition event callback.
        /// </summary>
        private void Internal_OnDead()
        {
            OnDead();
            OnDeadCallback?.Invoke();
        }

        /// <summary>
        /// Internal OnRevive method to call addition event callback.
        /// </summary>
        private void Internal_OnRevive()
        {
            OnRevive();
            OnReviveCallback?.Invoke();
        }
        #endregion

        #region [IHealth Implemetation]
        /// <summary>
        /// Get current health point.
        /// </summary>
        public override float GetHealth()
        {
            return health;
        }

        /// <summary>
        /// Alive state of health object.
        /// </summary>
        /// <returns>
        /// True if health > 0.
        /// Otherwise false.
        /// </returns>
        public override bool IsAlive()
        {
            return health > 0;
        }
        #endregion

        #region [IDamageable Implementation]
        /// <summary>
        /// Take damage to the health.
        /// </summary>
        /// <param name="amount">Damage amount.</param>
        public override void TakeDamage(float amount)
        {
            SetHealth(health - amount);
            OnTakeDamageCallback?.Invoke(amount);
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when object take damage.
        /// </summary>
        public event Action<float> OnTakeDamageCallback;

        /// <summary>
        /// Called when object is die.
        /// </summary>
        public event Action OnDeadCallback;

        /// <summary>
        /// Called when object is revive.
        /// </summary>
        public event Action OnReviveCallback;
        #endregion

        #region [Getter / Setter]
        public void SetHealth(float value)
        {
            float previousHealth = health;
            health = Mathf.Clamp(value, minHealth, maxHealth);
            if (!IsAlive())
            {
                Internal_OnDead();
            }
            else if(previousHealth == 0)
            {
                Internal_OnRevive();
            }
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetMaxHealth(float value)
        {
            maxHealth = value;
        }

        public float GetMinHealth()
        {
            return minHealth;
        }

        public void SetMinHealth(float value)
        {
            minHealth = value >= 0 ? value : 0;
        }
        #endregion
    }
}