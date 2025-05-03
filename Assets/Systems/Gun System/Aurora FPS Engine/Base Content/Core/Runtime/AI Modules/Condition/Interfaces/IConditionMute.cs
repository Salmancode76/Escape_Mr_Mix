/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.AIModules.Conditions
{
    public interface IConditionMute
    {
        /// <summary>
        /// Condition mute state value.
        /// If true this condition will be ignored.
        /// </summary>
        bool IsMuted();
    }
}