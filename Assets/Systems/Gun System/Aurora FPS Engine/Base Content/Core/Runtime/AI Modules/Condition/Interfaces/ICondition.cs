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
    public interface ICondition
    {
        /// <summary>
        /// Condition for translate to the next AI behaviour.
        /// </summary>
        bool IsExecuted();
    }
}