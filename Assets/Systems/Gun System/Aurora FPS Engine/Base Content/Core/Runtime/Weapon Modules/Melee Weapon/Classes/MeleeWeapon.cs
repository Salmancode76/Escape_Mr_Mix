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

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Melee/Melee Weapon")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class MeleeWeapon : MonoBehaviour
    {
        [SerializeField]
        [MinValue(0)]
        private float damage;

        [SerializeField]
        [MinValue(0.0f)]
        private float impulse;

        [SerializeField]
        private DecalMapping decalMapping;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterCallback?.Invoke(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollisionEnterCallback?.Invoke(collision);
        }

        #region [Event Callback Functions]
        internal event Action<Collider> OnTriggerEnterCallback;

        internal event Action<Collision> OnCollisionEnterCallback;
        #endregion

        #region [Getter / Setter]
        public float GetDamage()
        {
            return damage * WeaponUtilities.DamageMultiplier;
        }

        public void SetDamage(float value)
        {
            damage = value;
        }

        public float GetImpulse()
        {
            return impulse;
        }

        public void SetImpulse(float value)
        {
            impulse = value;
        }

        public DecalMapping GetDecalMapping()
        {
            return decalMapping;
        }

        public void SetDecalMapping(DecalMapping value)
        {
            decalMapping = value;
        }
        #endregion
    }
}
