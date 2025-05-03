/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    [System.Obsolete]
    public interface ICameraControl
    {
        /// <summary>
        /// Initialize camera control instance.
        /// </summary>
        /// <param name="controller">Target character controller reference.</param>
        void Initialize(Controller controller);
    }
}