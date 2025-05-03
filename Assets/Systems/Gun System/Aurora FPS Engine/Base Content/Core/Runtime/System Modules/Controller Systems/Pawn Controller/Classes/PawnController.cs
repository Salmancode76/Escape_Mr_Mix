/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.InputSystem;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class PawnController : Controller, IControlInput
    {
        // Stored required properties.
        private Vector2 inputVector = Vector2.zero;

        /// <summary>
        /// Called every frame, while the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            ReadInput();
        }

        /// <summary>
        /// Read input and save in Vector2D representation.
        /// </summary>
        protected virtual void ReadInput()
        {
            inputVector.x = InputReceiver.MovementHorizontalAction.ReadValue<float>();
            inputVector.y = InputReceiver.MovementVerticalAction.ReadValue<float>();
        }

        #region [IControllerInput Implementation]
        public Vector2 GetControlInput()
        {
            return inputVector;
        }
        #endregion
    }
}
