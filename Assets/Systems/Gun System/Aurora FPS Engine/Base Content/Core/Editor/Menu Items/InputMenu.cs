/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using System.IO;
using UnityEditor;
using UnityEngine.InputSystem;

namespace AuroraFPSEditor
{
    internal static class InputMenu
    {
        public const string RELATIVE_PATH = "Base Content/Resources/Input/InputMap.inputactions";

        [MenuItem("Aurora FPS Engine/Input/Open Input Map", false, 150)]
        public static void OpenInputMap()
        {
            string path = Path.Combine(ApexSettings.RootPath, RELATIVE_PATH);
            InputActionAsset inputMap = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
            if (inputMap != null)
            {
                AssetDatabase.OpenAsset(inputMap);
            }
        }
    }
}
