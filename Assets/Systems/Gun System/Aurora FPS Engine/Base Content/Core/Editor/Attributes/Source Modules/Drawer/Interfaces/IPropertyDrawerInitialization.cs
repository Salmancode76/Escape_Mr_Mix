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
    public interface IPropertyDrawerInitialization
    {
        /// <summary>
        /// Called once when initializing PropertyDrawer.
        /// </summary>
        /// <param name="property">Serialized property with DrawerAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        void OnInitialize(SerializedProperty property, GUIContent label);
    }
}