/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine;
using UnityEditor;

namespace AuroraFPSEditor.Attributes
{
    public interface IPropertyValidatorReceiver
    {
        /// <summary>
        /// Return true if this property valid the using with this attribute.
        /// If return false, this property attribute will be ignored.
        /// </summary>
        /// <param name="property">Reference of serialized property.</param>
        /// <param name="label">Display label of serialized property.</param>
        bool IsValidProperty(SerializedProperty property, GUIContent label);
    }
}