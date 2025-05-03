/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;
using System.Collections.Generic;
using System.Linq;
using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.SystemModules.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor
{
    internal sealed class SettingsSystemProvider : SettingsProvider
    {
        private SettingsConfig settingsConfig;
        private Editor editor;
        private GenericMenu settingsConfigMenu;
        private Type settingsConfigType;

        /// <summary>
        /// Settings system provider constructor.
        /// </summary>
        /// <param name="path">Path used to place the SettingsProvider in the tree view of the Settings window. The path should be unique among all other settings paths and should use "/" as its separator.</param>
        /// <param name="scopes">Scope of the SettingsProvider. The Scope determines whether the SettingsProvider appears in the Preferences window (SettingsScope.User) or the Settings window (SettingsScope.Project).</param>
        /// <param name="keywords">List of keywords to compare against what the user is searching for. When the user enters values in the search box on the Settings window, SettingsProvider.HasSearchInterest tries to match those keywords to this list.</param>
        public SettingsSystemProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        /// <summary>
        /// Use this function to implement a handler for when the user clicks on the Settings in the Settings window. You can fetch a settings Asset or set up UIElements UI from this function.
        /// </summary>
        /// <param name="searchContext">Search context in the search box on the Settings window.</param>
        /// <param name="rootElement">Root of the UIElements tree. If you add to this root, the SettingsProvider uses UIElements instead of calling SettingsProvider.OnGUI to build the UI. If you do not add to this VisualElement, then you must use the IMGUI to build the UI.</param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (EditorBuildSettings.TryGetConfigObject<SettingsConfig>(SettingsConfigEditor.BUILD_CONFIG_OBJECT_KEY, out SettingsConfig value))
            {
                settingsConfig = value;
            }
            else
            {
                settingsConfig = Resources.FindObjectsOfTypeAll<SettingsConfig>().FirstOrDefault();
                if (settingsConfig == null)
                {
                    settingsConfig = SettingsConfig.CreateInstance<JSONSettingsConfig>();


                }
            }

            settingsConfigMenu = new GenericMenu();
            IEnumerable<Type> types = ApexReflection.FindSubclassesOf<SettingsConfig>();
            foreach (Type type in types)
            {
                settingsConfigMenu.AddItem(new GUIContent(type.Name), false, () => settingsConfigType = type);
            }

            editor = Editor.CreateEditor(settingsConfig);
        }

        /// <summary>
        /// Use this function to draw the UI based on IMGUI. This assumes you haven't added any children to the rootElement passed to the OnActivate function.
        /// </summary>
        /// <param name="searchContext">Search context for the Settings window. Used to show or hide relevant properties.</param>
        public override void OnGUI(string searchContext)
        {
            if (settingsConfig != null && editor != null)
            {
                bool isNativeAsset = AssetDatabase.IsNativeAsset(settingsConfig);
                if (!isNativeAsset)
                {
                    Rect position = GUILayoutUtility.GetRect(0, 70);
                    position.x += 10;
                    position.y += 10;
                    position.width -= 15;

                    Rect popupPosition = new Rect(position.x, position.y, position.width - 155, 20);
                    if (GUI.Button(popupPosition, settingsConfigType?.Name ?? "Select config type...", EditorStyles.popup))
                    {
                        settingsConfigMenu.DropDown(popupPosition);
                    }

                    EditorGUI.BeginDisabledGroup(settingsConfigType == null);
                    Rect buttonPosition = new Rect(popupPosition.xMax, position.y, position.width - popupPosition.width, 18);
                    if (GUI.Button(buttonPosition, "Create Config", EditorStyles.miniButton))
                    {
                        SettingsConfig settingsSettings = SettingsConfig.CreateInstance(settingsConfigType) as SettingsConfig;

                        string path = AssetDatabase.GenerateUniqueAssetPath(ApexSettings.RootPath + string.Format("/New {0}.asset", settingsConfigType.Name));
                        AssetDatabase.CreateAsset(settingsSettings, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        EditorBuildSettings.AddConfigObject(SettingsConfigEditor.BUILD_CONFIG_OBJECT_KEY, settingsSettings, true);

                        EditorGUIUtility.PingObject(settingsSettings);
                    }
                    EditorGUI.EndDisabledGroup();

                    Rect helpBoxPosition = new Rect(position.x, buttonPosition.yMax + EditorGUIUtility.standardVerticalSpacing, position.width, 30);
                    EditorGUI.HelpBox(helpBoxPosition, "If you to edit settings config create new settings config asset.", MessageType.Info);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.Space(10);
                EditorGUI.BeginDisabledGroup(!isNativeAsset);
                editor.OnInspectorGUI();
                EditorGUI.EndDisabledGroup();

                GUILayout.EndVertical();
                GUILayout.Space(3);
                GUILayout.EndHorizontal();
            }
        }

        #region [Static Methods]
        /// <summary>
        /// Register settings system provider in project settings window.
        /// </summary>
        /// <returns>New settings system provider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider RegisterSettingsSystemProvider()
        {
            return new SettingsSystemProvider("Project/Aurora FPS Engine/Settings System", SettingsScope.Project);
        }
        #endregion
    }
}
