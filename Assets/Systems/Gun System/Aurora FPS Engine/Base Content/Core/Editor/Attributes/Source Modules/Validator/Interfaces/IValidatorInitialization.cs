/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public interface IValidatorInitialization
    {
        /// <summary>
        /// Called once when initializing PropertyValidator.
        /// </summary>
        /// <param name="property">Serialized property with ValidatorAttribute.</param>
        /// <param name="attribute">ValidatorAttribute of serialized property.</param>
        /// <param name="label">Label of serialized property.</param>
        void OnInitialize(SerializedProperty property, ValidatorAttribute attribute, GUIContent label);
    }
}