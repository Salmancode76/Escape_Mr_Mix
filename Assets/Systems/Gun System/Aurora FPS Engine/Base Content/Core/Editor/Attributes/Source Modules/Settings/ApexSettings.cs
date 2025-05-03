/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public sealed class ApexSettings : ScriptableObject
    {
        #region [Const Properties]
        public const string PATH_KEY = "Aurora FPS Editor Path";
        #endregion

        #region [Static Readonly Properties]
        public static readonly Color HeaderColorLightTheme = new Color(0.85f, 0.85f, 0.85f, 1.0f);
        public static readonly Color PropertyColorLightTheme = new Color(0.795f, 0.795f, 0.795f, 1.0f);
        public static readonly Color GroupBorderColorLightTheme = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        public static readonly Color PropertyBorderColorLightTheme = new Color(0.57f, 0.57f, 0.57f, 1.0f);
        public static readonly Color HeaderColorDarkTheme = new Color(0.275f, 0.275f, 0.275f, 1.0f);
        public static readonly Color PropertyColorDarkTheme = new Color(0.23f, 0.23f, 0.23f, 1.0f);
        public static readonly Color GroupBorderColorDarkTheme = Color.black;
        public static readonly Color PropertyBorderColorDarkTheme = Color.black;
        #endregion

        [System.Flags]
        public enum HighlightExpadablePropertyType
        {
            Disabled = 0,
            InsideGroup = 1 << 0,
            OutsideGroup = 1 << 1,
            All = ~0
        }

        [SerializeField]
        private bool apexEnabled;

        [SerializeField]
        private string[] exceptScripts;

        [SerializeField]
        private string[] defaultTypes;

        [SerializeField]
        private bool debugMode = false;

        [SerializeField]
        private bool readmeAtStartup;

        /// <summary>
        /// Get actual Apex settings asset.
        /// </summary>
        public static ApexSettings ActualSettings
        {
            get
            {
                string relativePath = "Base Content/Core/Editor/Editor Resources/Scriptable Data";
                string path = Path.Combine(RootPath, relativePath, "EditorSettings.asset");
                FileInfo settingsAsset = new FileInfo(path);
                if (!settingsAsset.Exists)
                {
                    settingsAsset.Directory.Create();
                }

                ApexSettings settings = AssetDatabase.LoadAssetAtPath<ApexSettings>(path);
                if (settings == null)
                {
                    settings = CreateInstance<ApexSettings>();
                    ResetSettings(ref settings);
                    AssetDatabase.CreateAsset(settings, path);
                    AssetDatabase.SaveAssets();
                }
                return settings;
            }
        }

        /// <summary>
        /// Get serialized actual Apex settings in SerializedObject representation.
        /// </summary>
        public static SerializedObject SerializedActualSettings
        {
            get
            {
                return new SerializedObject(ActualSettings);
            }
        }

        /// <summary>
        /// Reset specific settings to default.
        /// </summary>
        /// <param name="settings">Settings reference.</param>
        public static void ResetSettings(ref ApexSettings settings)
        {
            settings.ApexEnabled(true);
            settings.DebugMode(false);
            settings.ShowReadmeAtStartup(true);
            settings.SetDefaultTypes(new string[1] { "InputAction" });
        }

        #region [Static Properties]
        /// <summary>
        /// Get actual header color regarding current Unity editor theme.
        /// </summary>
        public static Color HeaderColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? HeaderColorDarkTheme : HeaderColorLightTheme;
            }
        }

        /// <summary>
        /// Get actual property color regarding current Unity editor theme.
        /// </summary>
        public static Color PropertyColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? PropertyColorDarkTheme : PropertyColorLightTheme;
            }
        }

        /// <summary>
        /// Get actual group border color regarding current Unity editor theme.
        /// </summary>
        public static Color GroupBorderColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? GroupBorderColorDarkTheme : GroupBorderColorLightTheme;
            }
        }

        /// <summary>
        /// Get actual property border color regarding current Unity editor theme.
        /// </summary>
        public static Color PropertyBorderColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? PropertyBorderColorDarkTheme : PropertyBorderColorLightTheme;
            }
        }

        public static string RootPath
        {
            get
            {
                if (!EditorPrefs.HasKey(PATH_KEY))
                {
                    EditorPrefs.SetString(PATH_KEY, "Assets/Aurora FPS Engine");
                }
                return EditorPrefs.GetString(PATH_KEY);
            }
        }
        #endregion

        #region [Getter / Setter]
        public bool ApexEnabled()
        {
            return apexEnabled;
        }

        public void ApexEnabled(bool value)
        {
            apexEnabled = value;
        }

        public bool DebugMode()
        {
            return debugMode;
        }

        public void DebugMode(bool value)
        {
            debugMode = value;
        }

        public string[] GetExceptScripts()
        {
            return exceptScripts;
        }

        public void SetExceptScripts(string[] value)
        {
            exceptScripts = value;
        }

        public string[] GetDefaultTypes()
        {
            return defaultTypes;
        }

        public void SetDefaultTypes(string[] value)
        {
            defaultTypes = value;
        }

        public bool ShowReadmeAtStartup()
        {
            return readmeAtStartup;
        }

        public void ShowReadmeAtStartup(bool value)
        {
            readmeAtStartup = value;
        }
        #endregion
    }
}