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
    public interface IInverseKinematic
    {
        /// <summary>
        /// Return true if IK is processing, otherwise false.
        /// </summary>
        bool IsActive();
    }
}