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
    [System.Obsolete("Use Controller from AuroraFPSRuntime.SystemModules.ControllerSystem instead.")]
    public interface IControllerState
    {
        ControllerState GetState();

        bool CompareState(ControllerState value);

        bool HasState(ControllerState value);
    }
}