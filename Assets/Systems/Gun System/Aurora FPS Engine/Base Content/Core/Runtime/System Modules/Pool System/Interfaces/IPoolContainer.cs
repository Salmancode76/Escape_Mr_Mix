/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules
{
    public interface IPoolContainer
    {
        /// <summary>
        /// Length of available object in pool.
        /// </summary>
        int GetLength();
    }
}