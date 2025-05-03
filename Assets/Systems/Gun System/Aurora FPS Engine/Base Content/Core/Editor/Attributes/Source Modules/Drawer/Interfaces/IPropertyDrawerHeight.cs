/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public interface IPropertyDrawerHeight
    {
        /// <summary>
        /// Get height which needed to draw property.
        /// </summary>
        /// <param name="property">Serialized property with DrawerAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        float GetPropertyHeight(SerializedProperty property, GUIContent label);
    }
}