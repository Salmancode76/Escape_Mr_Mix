/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime
{
    public interface IControllerSpeed
    {
        float GetSpeed();

        float GetWalkSpeed();

        float GetRunSpeed();

        float GetSprintSpeed();

        float GetBackwardSpeedPerсent();

        float GetSideSpeedPerсent();
    }
}