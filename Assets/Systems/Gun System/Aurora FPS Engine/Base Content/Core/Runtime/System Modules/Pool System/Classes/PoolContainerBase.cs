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
    [System.Serializable]
    public abstract class PoolContainerBase : IPoolContainer
    {
        public enum Allocator
        {
            Free,
            Fixed,
            Dynamic
        }

        /// <summary>
        /// Length of available object in pool.
        /// </summary>
        public abstract int GetLength();
    }
}