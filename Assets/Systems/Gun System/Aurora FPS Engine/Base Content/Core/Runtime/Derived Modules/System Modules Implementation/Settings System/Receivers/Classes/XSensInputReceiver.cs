﻿/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Receivers/X Sens Input Settings Receiver")]
    [DisallowMultipleComponent]
    public sealed class XSensInputReceiver : SettingsReceiver
    {
        [SerializeField]
        [NotNull]
        private PawnCamera pawnCamera;

        [SerializeField]
        private float defaultValue = 15.0f;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(pawnCamera != null, $"<b><color=#FF0000>Attach reference of the player camera to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Pawn Camera<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Load</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            Vector2 sensitivity = pawnCamera.GetSensitivity();
            sensitivity.x = (float)value;
            pawnCamera.SetSensitivity(sensitivity);
        }

        /// <summary>
        /// <br>Called when settings file is not found or 
        /// target processor GUID is not found in loaded buffer.</br>
        /// <br>Implement this method to determine default value for this processor.</br>
        /// </summary>
        /// <returns>Default value of processor.</returns>
        public override object GetDefaultValue()
        {
            return defaultValue;
        }
    }
}