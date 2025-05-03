/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.CommandLine;
using UnityEditor;

namespace AuroraFPSEditor.Utilities
{
    internal static class ToolsMenu
    {
        [MenuItem("Aurora FPS Engine/Create/Tools/Console", false, 104)]
        [MenuItem("GameObject/Aurora FPS Engine/Tools/Console", false, 7)]
        private static void CreateConsole()
        {
            Console console = Console.GetRuntimeInstance();
            EditorGUIUtility.PingObject(console);
            Selection.activeObject = console;
        }
    }
}
