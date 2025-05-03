/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.SystemModules.ControllerModules;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;

namespace AuroraFPSRuntime
{
    [HideScriptField]
    [DisallowMultipleComponent]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Health/First Person Health")]
    public partial class FPHealth : CharacterHealth
    {
        // Base character health properties.
        [SerializeField]
        [ReorderableList(
            DisplayHeader = false, 
            ElementLabel = "Settings {niceIndex}",
            NoneElementLabel = "Add new settings...")]
        [Foldout("Velocity Damage Settings", Style = "Header")]
        private VelocityDamage[] velocityDamages;

        [SerializeField]
        [ReorderableList(ElementLabel = "Preset {niceIndex}", DisplayHeader = false)]
        [Foldout("Damage Shake Settings", Style = "Header")]
        private DamageShake[] damageShakes;

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            for (int i = 0; i < damageShakes.Length; i++)
            {
                DamageShake damageShake = damageShakes[i];
                if(Math.InRange(amount, damageShake.GetDamageRange()))
                {
                    CameraShaker.GetRuntimeInstance().RegisterShake(new KickShake(damageShake.GetShakeSettings(), damageShake.GetShakeDirection()));
                }
            }
        }

        #region [Velocity Damage Implementation]
        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun
        /// touching another rigidbody/collider.
        /// </summary>
        /// <param name="other">The Collision data associated with this collision.</param>
        protected virtual void OnCollisionEnter(Collision other)
        {
            CalculateVelocityDamage(other.relativeVelocity.magnitude);
        }

        /// <summary>
        /// Calculate and apply damage by velocity.
        /// </summary>
        protected virtual void CalculateVelocityDamage(float velocity)
        {
            if (enabled && velocity > 0)
            {
                for (int i = 0; i < velocityDamages.Length; i++)
                {
                    VelocityDamage property = velocityDamages[i];
                    if (Math.InRange(velocity, property.GetVelocity()))
                    {
                        TakeDamage(property.GetDamage());
                    }
                }
            }
        }
        #endregion

        #region [Getter / Setter]
        public VelocityDamage[] GetVelocityDamageProperties()
        {
            return velocityDamages;
        }

        public void SetVelocityDamageProperties(VelocityDamage[] value)
        {
            velocityDamages = value;
        }
        #endregion
    }
}