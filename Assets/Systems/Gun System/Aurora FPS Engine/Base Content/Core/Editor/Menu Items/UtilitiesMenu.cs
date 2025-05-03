/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS
   Publisher :    Tamerlan Global Inc.
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSEditor.Utilities;
using System.IO;
using UnityEditor;

namespace AuroraFPSEditor
{
    internal static class UtilitiesMenu
    {
        [MenuItem("Aurora FPS Engine/Utilities/Integrations", priority = 199)]
        public static void OpenIntegrations()
        {
            SettingsService.OpenProjectSettings("Project/Aurora FPS Engine/Integrations");
        }

        [MenuItem("Aurora FPS Engine/Utilities/Remote Controller Sync", priority = 201)]
        public static void OpenRemoteControllerSync()
        {
            RemoteControllerSync window = EditorWindow.GetWindow<RemoteControllerSync>(true, "Remote Controller Sync", true);
            window.minSize = RemoteControllerSync.WindowSize;
            window.maxSize = RemoteControllerSync.WindowSize;
            window.Show();
            EditorUtility.DisplayDialog("Remote Controller Sync", "Please note: This utility only works with the standard Animator Controller for remote body.", "Continue");
        }

        //[MenuItem("Aurora FPS Engine/Utilities/Install Project Settings", false, 999)]
        public static void InstallProjectSettings()
        {
            const string RELATIVE_PATH = "/Base Content/Core/Editor/Editor Resources/Library Assets/ProjectSettings.unitypackage";
            string path = Path.Combine(ApexSettings.RootPath + RELATIVE_PATH);
            AssetDatabase.ImportPackage(path, false);
        }

        //[MenuItem("Aurora FPS Engine/Utilities/Export Project Settings", false, 999)]
        internal static void ExportProjectSettings()
        {
            AssetDatabase.ExportPackage("", "INTERNAL_PROJECT_SETTINGS.unitypackage", ExportPackageOptions.IncludeLibraryAssets);
        }
    }
}
