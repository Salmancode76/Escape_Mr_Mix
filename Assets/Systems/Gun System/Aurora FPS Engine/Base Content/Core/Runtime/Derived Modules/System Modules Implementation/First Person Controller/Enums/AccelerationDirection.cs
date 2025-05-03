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
    /// First person controller sprint directions.
    /// </summary>
    [Flags]
    [System.Obsolete("Use Controller from AuroraFPSRuntime.SystemModules.ControllerSystem instead.")]
    public enum AccelerationDirection
    {
        /// <summary>
        /// Sprint movement disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        ///  Sprint movement available only on forward direction.
        /// </summary>
        Forward = 1 << 0,

        /// <summary>
        /// Sprint movement available only on side directions.
        /// </summary>
        Side = 1 << 1,

        /// <summary>
        /// Sprint movement available only on backword direction.
        /// </summary>
        Backword = 1 << 2,

        /// <summary>
        /// Sprint movement available on any directions.
        /// </summary>
        All = ~0
    }
}