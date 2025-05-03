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
    public interface IControllerDestination
    {
        /// <summary>
        /// Set controller destination.
        /// </summary>
        /// <param name="position">Position in wolrd space.</param>
        void SetDestination(Vector3 position);

        /// <summary>
        /// Return true if controller reach current destination. Otherwise false.
        /// </summary>
        bool IsReachDestination();
    }
}