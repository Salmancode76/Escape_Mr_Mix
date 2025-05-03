/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor
{
    [CustomEditor(typeof(InputConfig), true)]
    internal class InputConfigEditor : ApexEditor
    {
        public const string BUILD_CONFIG_OBJECT_KEY = "Global Input Config Asset Key";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!AssetDatabase.IsNativeAsset(target) && EditorBuildSettings.TryGetConfigObject<InputConfig>(BUILD_CONFIG_OBJECT_KEY, out InputConfig value))
            {
                if (value != target && GUILayout.Button("Make as Global Config", GUILayout.Height(30)))
                {
                    EditorBuildSettings.AddConfigObject(BUILD_CONFIG_OBJECT_KEY, target, true);
                }
            }
        }
    }
}