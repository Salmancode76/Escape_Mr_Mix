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

namespace AuroraFPSRuntime.WeaponModules
{
    [System.Serializable]
    public abstract class WeaponEffect
    {
        [SerializeField]
        [NotNull]
        protected Transform hinge;

        /// <summary>
        /// Called once when the script instance is being loaded.
        /// </summary>
        /// <param name="weapon">Weapon transform reference.</param>
        public virtual void Initialize(Transform weapon)
        {

        }

        public abstract void OnUpdate();

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

        #region [Getter / Setter]
        public Transform GetHinge()
        {
            return hinge;
        }

        public void SetHinge(Transform value)
        {
            hinge = value;
        }
        #endregion
    }
}