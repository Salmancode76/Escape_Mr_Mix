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
    public interface IPainterGUI
    {
        /// <summary>
        /// Called for rendering and handling GUI events.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the painter GUI.</param>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        void OnPainterGUI(Rect position, SerializedProperty property, GUIContent label);
    }
}