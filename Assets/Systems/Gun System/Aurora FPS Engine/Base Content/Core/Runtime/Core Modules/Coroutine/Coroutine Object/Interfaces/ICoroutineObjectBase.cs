/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.CoreModules.Coroutines
{
    public interface ICoroutineObjectBase
    {
        /// <summary>
        /// Coroutine is processing.
        /// </summary>
        bool IsProcessing();
    }
}