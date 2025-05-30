﻿/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace AuroraFPSEditor
{
    internal sealed class InputSystemProvider : SettingsProvider
    {
        private InputConfig config;
        private Editor editor;

        /// <summary>
        /// Input system provider constructor.
        /// </summary>
        /// <param name="path">Path used to place the SettingsProvider in the tree view of the Settings window. The path should be unique among all other settings paths and should use "/" as its separator.</param>
        /// <param name="scopes">Scope of the SettingsProvider. The Scope determines whether the SettingsProvider appears in the Preferences window (SettingsScope.User) or the Settings window (SettingsScope.Project).</param>
        /// <param name="keywords">List of keywords to compare against what the user is searching for. When the user enters values in the search box on the Settings window, SettingsProvider.HasSearchInterest tries to match those keywords to this list.</param>
        public InputSystemProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        /// <summary>
        /// Use this function to implement a handler for when the user clicks on the Settings in the Settings window. You can fetch a settings Asset or set up UIElements UI from this function.
        /// </summary>
        /// <param name="searchContext">Search context in the search box on the Settings window.</param>
        /// <param name="rootElement">Root of the UIElements tree. If you add to this root, the SettingsProvider uses UIElements instead of calling SettingsProvider.OnGUI to build the UI. If you do not add to this VisualElement, then you must use the IMGUI to build the UI.</param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (EditorBuildSettings.TryGetConfigObject<InputConfig>(InputConfigEditor.BUILD_CONFIG_OBJECT_KEY, out InputConfig value))
            {
                config = value;
            }
            else
            {
                config = Resources.FindObjectsOfTypeAll<InputConfig>().FirstOrDefault();
                if (config == null)
                {
                    config = ScriptableObject.CreateInstance<InputConfig>();


                }
            }
            editor = Editor.CreateEditor(config);
        }

        /// <summary>
        /// Use this function to draw the UI based on IMGUI. This assumes you haven't added any children to the rootElement passed to the OnActivate function.
        /// </summary>
        /// <param name="searchContext">Search context for the Settings window. Used to show or hide relevant properties.</param>
        public override void OnGUI(string searchContext)
        {
            if (config != null && editor != null)
            {
                bool isNativeAsset = AssetDatabase.IsNativeAsset(config);

                if (!isNativeAsset)
                {
                    Rect position = GUILayoutUtility.GetRect(0, 90);
                    position.x += 10;
                    position.y += 10;
                    position.width -= 15;

                    Rect helpBoxPosition = new Rect(position.x, position.y, position.width, 35);
                    EditorGUI.HelpBox(helpBoxPosition, "If you want to edit the input config create new input config asset.", MessageType.Info);

                    Rect buttonPosition = new Rect(position.x, helpBoxPosition.yMax + EditorGUIUtility.standardVerticalSpacing, position.width, 35);
                    if (GUI.Button(buttonPosition, "Create Config"))
                    {
                        InputConfig config = ScriptableObject.CreateInstance<InputConfig>();

                        string path = AssetDatabase.GenerateUniqueAssetPath(ApexSettings.RootPath + string.Format("/New {0}.asset", config.GetType().Name));
                        AssetDatabase.CreateAsset(config, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        EditorBuildSettings.AddConfigObject(InputConfigEditor.BUILD_CONFIG_OBJECT_KEY, config, true);

                        EditorGUIUtility.PingObject(config);
                    }
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.Space(10);
                EditorGUIUtility.labelWidth = 248;

                EditorGUI.BeginDisabledGroup(!isNativeAsset);
                editor.OnInspectorGUI();
                EditorGUI.EndDisabledGroup();

                GUILayout.EndVertical();
                GUILayout.Space(5);
                GUILayout.EndHorizontal();
            }
        }

        #region [Static Methods]
        /// <summary>
        /// Register input system provider in project settings window.
        /// </summary>
        /// <returns>New input system provider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider RegisterInputSystemProvider()
        {
            return new InputSystemProvider("Project/Aurora FPS Engine/Input System", SettingsScope.Project);
        }
        #endregion
    }
}
