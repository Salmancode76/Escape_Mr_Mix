/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    /// <summary>
    /// First person controller air control types.
    /// </summary>
    [Flags]
    [System.Obsolete("Use Controller from AuroraFPSRuntime.SystemModules.ControllerSystem instead.")]
    public enum AirControl
    {
        /// <summary>
        /// Controller air control is disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Controller air control handled only by input.
        /// </summary>
        Input = 1 << 0,

        /// <summary>
        /// Controller air control handled only by camera direction.
        /// </summary>
        Camera = 1 << 1,

        /// <summary>
        /// Controller air control handled by input and camera direction.
        /// </summary>
        Both = ~0
    }
}