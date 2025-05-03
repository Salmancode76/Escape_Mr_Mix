/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    interface IPlayerState
    {
        ControllerState GetState();
        bool CompareState(ControllerState state);
        bool HasState(ControllerState state);
    }
}
