/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    public interface IControllerVelocity
    {
        /// <summary>
        /// Controller velocity in Vector3 representation.
        /// </summary>
        Vector3 GetVelocity();
    }
}