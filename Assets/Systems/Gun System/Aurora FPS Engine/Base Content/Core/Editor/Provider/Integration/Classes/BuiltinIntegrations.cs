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
using UnityEngine;
using UnityEditor;

namespace AuroraFPSEditor.Internal.Integrations
{
    [Integration("Emerald AI 3.0")]
    public sealed class EmeraldAIIntegrationEditor : IntegrationEditor
    {
        private readonly string PackagePath = Path.Combine(ApexSettings.RootPath, "Base Content/Core/Editor/Editor Resources/Library Assets/Integrations/Emerald AI 3.0.unitypackage");
        private readonly string InstalledPath = Path.Combine(ApexSettings.RootPath, "Integrations/Emerald AI 3.0");

        private bool hasPackage;
        private bool isInstalled;

        public EmeraldAIIntegrationEditor()
        {
            hasPackage = File.Exists(PackagePath);
            isInstalled = Directory.Exists(InstalledPath);
        }

        public override void OnGUI(Rect position)
        {
            if (!isInstalled)
            {
                EditorGUI.BeginDisabledGroup(!hasPackage);
                if (RightButton(ref position, "Install"))
                {
                    if (EditorUtility.DisplayDialog("Attention", "Before installing the add-on, make sure that Emerald AI is installed in your project. " +
                "Otherwise, you will get a compilation error.", "Continue", "Cancel"))
                    {
                        AssetDatabase.ImportPackage(PackagePath, false);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (RightButton(ref position, "Uninstall"))
                {
                    if (EditorUtility.DisplayDialog("Attention", "Are you really want to delete Emerald AI 3.0 add-on?", "Yes", "No"))
                    {
                        if (Directory.Exists(InstalledPath))
                        {
                            Directory.Delete(InstalledPath, true);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    }
                }
            }
        }
    }
}