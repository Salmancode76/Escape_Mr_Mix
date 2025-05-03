/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine;
using AuroraFPSRuntime.Attributes;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(VisualClampAttribute))]
    internal sealed class VisualClampView : PropertyView
    {
        private VisualClampAttribute visualClampAttribute;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            visualClampAttribute = viewAttribute as VisualClampAttribute;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float sliderValue = EditorGUI.Slider(position, label, Mathf.InverseLerp(visualClampAttribute.minValue, visualClampAttribute.maxValue, property.floatValue), 0, 1);
            property.floatValue = Mathf.Lerp(visualClampAttribute.minValue, visualClampAttribute.maxValue, sliderValue);
        }
    }
}