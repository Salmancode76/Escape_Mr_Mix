/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Health/Hitbox")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class Hitbox : MonoBehaviour, IHitbox, IHealth, IDamageable
    {
        [SerializeField]
        [NotNull]
        private HealthComponent targetHealth;

        [SerializeField]
        private float multiplier = 0;

        [SerializeField] 
        private float protection = 25;


        /// <summary>
        /// Initialization of hit box component.
        /// </summary>
        /// <param name="healthComponent">Health component reference.</param>
        public void Initialize(HealthComponent targetHealth)
        {
            this.targetHealth = targetHealth;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            enabled = targetHealth != null;
        }

        #region [IHitbox Implementation]
        public HealthComponent GetTargetHealth()
        {
            return targetHealth;
        }
        #endregion

        #region [IHealth Implementation]
        /// <summary>
        /// Health of target health component.
        /// </summary>
        public float GetHealth()
        {
            return targetHealth.GetHealth();
        }

        /// <summary>
        /// Is alive state of target health component.
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return targetHealth.IsAlive();
        }
        #endregion

        #region [IDamageable Implementation]
        /// <summary>
        /// Take damage to the health component.
        /// </summary>
        /// <param name="amount">Damage amount value.</param>
        public void TakeDamage(float amount)
        {
            if (targetHealth != null)
            {
                amount = Mathf.Abs(amount);

                if (protection > 0)
                {
                    protection -= amount;
                    if (protection < 0)
                    {
                        targetHealth.TakeDamage(Mathf.Abs(protection));
                        protection = 0;
                    }
                    else
                    {
                        targetHealth.TakeDamage(0);
                    }
                }
                else if (protection == 0)
                {
                    targetHealth.TakeDamage(amount + multiplier);
                }
            }
        }
        #endregion

        #region [Getter / Setter]
        public float GetMultiplier()
        {
            return multiplier;
        }

        public void SetMultiplier(float value)
        {
            multiplier = value;
        }

        public float GetProtection()
        {
            return protection;
        }

        public void SetProtection(float value)
        {
            protection = value;
        }
        #endregion
    }
}

