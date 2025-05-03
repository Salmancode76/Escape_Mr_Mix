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
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace AuroraFPSEditor.Attributes
{
    [CustomEditor(typeof(ApexSettings))]
    public class ApexSettingsEditor : Editor
    {
        private string rootPath;
        private SerializedProperty apexEnabled;
        private ReorderableList exceptScripts;
        private ReorderableList defaultTypes;
        private SerializedProperty debugMode;
        private SerializedProperty readmeAtStartup;

        private void OnEnable()
        {
            rootPath = EditorPrefs.GetString(ApexSettings.PATH_KEY);
            apexEnabled = serializedObject.FindProperty(nameof(apexEnabled));
            debugMode = serializedObject.FindProperty(nameof(debugMode));
            readmeAtStartup = serializedObject.FindProperty(nameof(readmeAtStartup));
            exceptScripts = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(exceptScripts)));
            defaultTypes = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(defaultTypes)));
            exceptScripts.headerHeight = 1;
            exceptScripts.drawElementCallback += (position, index, isActive, isFocused) =>
            {
                SerializedProperty element = exceptScripts.serializedProperty.GetArrayElementAtIndex(index);
                position.y += 1;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, element, GUIContent.none);
            };
            defaultTypes.headerHeight = 1;
            defaultTypes.drawElementCallback += (position, index, isActive, isFocused) =>
            {
                SerializedProperty element = defaultTypes.serializedProperty.GetArrayElementAtIndex(index);
                position.y += 1;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, element, GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            rootPath = EditorGUILayout.DelayedTextField("Root Path", rootPath);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(ApexSettings.PATH_KEY, rootPath);
            }

            EditorGUILayout.PropertyField(apexEnabled, new GUIContent("Attributes Enabled"));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(debugMode, new GUIContent("Debug Mode"));
            if (EditorGUI.EndChangeCheck())
            {
                string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                List<string> allDefines = definesString.Split(';').ToList();
                if (debugMode.boolValue)
                    allDefines.Add("AURORA_ENGINE_DEBUG");
                else
                    allDefines.RemoveAll(v => v == "AURORA_ENGINE_DEBUG");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
            }

            exceptScripts.serializedProperty.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(exceptScripts.serializedProperty.isExpanded, "Except Script");
            if (exceptScripts.serializedProperty.isExpanded)
            {
                exceptScripts.DoLayoutList();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            defaultTypes.serializedProperty.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(defaultTypes.serializedProperty.isExpanded, "Default Editor");
            if (defaultTypes.serializedProperty.isExpanded)
            {
                defaultTypes.DoLayoutList();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
