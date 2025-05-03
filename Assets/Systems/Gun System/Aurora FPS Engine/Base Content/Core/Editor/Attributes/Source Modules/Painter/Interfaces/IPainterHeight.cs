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
    public interface IPainterHeight
    {
        /// <summary>
        /// Get the height of the decorator, which required to display it.
        /// Calculate only the size of the current decorator, not the entire property.
        /// The decorator height will be added to the total size of the property with other decorators.
        /// </summary>
        /// <param name="property">Reference of serialized property decorator attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        float GetPainterHeight(SerializedProperty property, GUIContent label);
    }
}