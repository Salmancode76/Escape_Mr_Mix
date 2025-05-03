/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Runtime/System Modules/Hardware/Cursor/Cursor Handler")]
    [DisallowMultipleComponent]
    public sealed class CursorHandler : MonoBehaviour
    {
        private enum Action
        {
            Show,
            Hide
        }

        [SerializeField]
        private Action action = Action.Show;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            switch (action)
            {
                case Action.Show:
                    InputReceiver.HardwareCursor(true);
                    break;
                case Action.Hide:
                    InputReceiver.HardwareCursor(false);
                    break;
            }
        }
    }
}