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
    public interface IPropertyDrawerGUI
    {
        /// <summary>
        /// Called for drawing property view GUI.
        /// </summary>
        /// <param name="position">Position of the serialized property.</param>
        /// <param name="property">Serialized property with DrawerAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        void OnGUI(Rect position, SerializedProperty property, GUIContent label);
    }
}