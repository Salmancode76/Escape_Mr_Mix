/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.Internal.Integrations
{
    internal sealed class IntegrationsProvider : SettingsProvider
    {
        private struct IntegrationInfo
        {
            public readonly IntegrationAttribute TargetAttribute;
            public readonly IntegrationEditor TargetEditor;

            public IntegrationInfo(IntegrationAttribute targetAttribute, IntegrationEditor targetEditor)
            {
                TargetAttribute = targetAttribute;
                TargetEditor = targetEditor;
            }
        }

        private List<IntegrationInfo> integrationInfos;

        internal static class ContentProperties
        {
            public static readonly Color BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1.0f);
            public static readonly Color BorderColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            public static readonly Color SeparatorColor = new Color(0.35f, 0.35f, 0.35f, 1.0f);
        }

        /// <summary>
        /// Integrations provider constructor.
        /// </summary>
        /// <param name="path">Path used to place the SettingsProvider in the tree view of the Settings window. The path should be unique among all other settings paths and should use "/" as its separator.</param>
        /// <param name="scopes">Scope of the SettingsProvider. The Scope determines whether the SettingsProvider appears in the Preferences window (SettingsScope.User) or the Settings window (SettingsScope.Project).</param>
        /// <param name="keywords">List of keywords to compare against what the user is searching for. When the user enters values in the search box on the Settings window, SettingsProvider.HasSearchInterest tries to match those keywords to this list.</param>
        public IntegrationsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        /// <summary>
        /// Use this function to implement a handler for when the user clicks on the Settings in the Settings window. You can fetch a settings Asset or set up UIElements UI from this function.
        /// </summary>
        /// <param name="searchContext">Search context in the search box on the Settings window.</param>
        /// <param name="rootElement">Root of the UIElements tree. If you add to this root, the SettingsProvider uses UIElements instead of calling SettingsProvider.OnGUI to build the UI. If you do not add to this VisualElement, then you must use the IMGUI to build the UI.</param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            IEnumerable<Type> types = ApexReflection.FindSubclassesOf<IntegrationEditor>();
            integrationInfos = new List<IntegrationInfo>();
            foreach (Type type in types)
            {
                IntegrationAttribute integrationAttribute = Attribute.GetCustomAttribute(type, typeof(IntegrationAttribute)) as IntegrationAttribute;
                if (integrationAttribute != null)
                {
                    integrationInfos.Add(new IntegrationInfo(integrationAttribute, Activator.CreateInstance(type) as IntegrationEditor));
                }
            }
        }

        /// <summary>
        /// Use this function to draw the UI based on IMGUI. This assumes you haven't added any children to the rootElement passed to the OnActivate function.
        /// </summary>
        /// <param name="searchContext">Search context for the Settings window. Used to show or hide relevant properties.</param>
        public override void OnGUI(string searchContext)
        {
            Rect position = GUILayoutUtility.GetRect(0, 0);
            position.x += 10;
            position.y += 10;
            position.width -= 20;

            if (integrationInfos != null && integrationInfos.Count > 0)
            {
                position.height = 0;
                for (int i = 0; i < integrationInfos.Count; i++)
                {
                    position.height += integrationInfos[i].TargetEditor.GetHeight();
                }
                position.height += 0.75f;
                DrawBackground(position);

                Rect elementPosition = new Rect(position.x, position.y + 0.75f, position.width, position.height);
                for (int i = 0; i < integrationInfos.Count; i++)
                {
                    IntegrationAttribute attribute = integrationInfos[i].TargetAttribute;
                    IntegrationEditor editor = integrationInfos[i].TargetEditor;

                    if (editor != null)
                    {
                        float height = editor.GetHeight();

                        GUIContent nameContent = new GUIContent(attribute.Name);
                        GUIStyle style = GUI.skin.label;
                        Rect namePosition = new Rect(elementPosition.x + 5, elementPosition.y, style.CalcSize(nameContent).x + 5, height);
                        GUI.Label(namePosition, nameContent);

                        Rect guiPosition = new Rect(namePosition.xMax, elementPosition.y, elementPosition.width - namePosition.width - 5, height);
                        editor.OnGUI(guiPosition);

                        elementPosition.y += height;

                        Rect linePosition = new Rect(elementPosition.x, elementPosition.y, elementPosition.width + 1, 0.75f);
                        EditorGUI.DrawRect(linePosition, ContentProperties.BorderColor);
                    }
                }
            }
            else
            {
                position.height = 20;
                GUI.Label(position, "List is empty.");
            }
        }

        private void DrawBackground(Rect position)
        {
            Rect backgroundPosition = new Rect(position.x, position.y, position.width, position.height);
            EditorGUI.DrawRect(backgroundPosition, ContentProperties.BackgroundColor);

            Rect topBorderPosition = new Rect(backgroundPosition.xMin, backgroundPosition.yMin, backgroundPosition.width + 0.5f, 0.75f);
            EditorGUI.DrawRect(topBorderPosition, ContentProperties.BorderColor);

            Rect bottomBorderPosition = new Rect(backgroundPosition.xMin, backgroundPosition.yMax, backgroundPosition.width + 0.5f, 0.75f);
            EditorGUI.DrawRect(bottomBorderPosition, ContentProperties.BorderColor);

            Rect leftBorderPosition = new Rect(backgroundPosition.xMin, backgroundPosition.yMin, 0.75f, backgroundPosition.height);
            EditorGUI.DrawRect(leftBorderPosition, ContentProperties.BorderColor);

            Rect rightBorderPosition = new Rect(backgroundPosition.xMax, backgroundPosition.yMin, 0.75f, backgroundPosition.height);
            EditorGUI.DrawRect(rightBorderPosition, ContentProperties.BorderColor);
        }

        #region [Static Methods]
        /// <summary>
        /// Register IntegrationsProvider in Project Settings window.
        /// </summary>
        /// <returns>New IntegrationsProvider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider RegisterIntegrationProvider()
        {
            return new IntegrationsProvider("Project/Aurora FPS Engine/Integrations", SettingsScope.Project);
        }
        #endregion
    }
}