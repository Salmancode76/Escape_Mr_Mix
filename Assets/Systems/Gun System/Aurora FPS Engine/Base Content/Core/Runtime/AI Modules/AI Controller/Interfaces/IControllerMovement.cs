/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.AIModules
{
    public interface IControllerMovement
    {
        /// <summary>
        /// Resume or stop controller movement.
        /// </summary>
        /// <param name="value">Set true to resume moving or false to stop.</param>
        void IsMoving(bool value);
    }
}