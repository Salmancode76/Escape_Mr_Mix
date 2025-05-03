/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.SystemModules.ControllerSystems;

namespace AuroraFPSRuntime.SystemModules.CameraSystems.Effects
{
    [System.Serializable]
    public abstract class PlayerCameraEffect
    {
        /// <summary>
        /// Implement this method to make some initialization 
        /// or get access to Controller and CameraControl references.
        /// </summary>
        /// <param name="controller">Player controller reference.</param>
        /// <param name="cameraControl">Player camera control reference.</param>
        public abstract void Initialization(PlayerController controller, PlayerCamera cameraControl);
    }
}