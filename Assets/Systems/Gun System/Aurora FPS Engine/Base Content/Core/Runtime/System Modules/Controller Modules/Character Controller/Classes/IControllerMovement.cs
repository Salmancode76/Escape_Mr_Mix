/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    [System.Obsolete("Use Controller from AuroraFPSRuntime.SystemModules.ControllerSystem instead.")]
    public interface IControllerMovement
    {
        Vector3 GetMovementDirection();
    }
}