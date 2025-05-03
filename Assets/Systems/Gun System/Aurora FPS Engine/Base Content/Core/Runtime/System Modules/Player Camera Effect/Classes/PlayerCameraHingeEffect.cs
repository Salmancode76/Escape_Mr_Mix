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

namespace AuroraFPSRuntime.SystemModules.CameraSystems.Effects
{
    [System.Serializable]
    public abstract class PlayerCameraHingeEffect : PlayerCameraEffect
    {
        [SerializeField]
        [NotNull]
        protected Transform hinge;

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