/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov, Alexandra Averyanova
   ---------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu(null)]
    public abstract class SettingsProcessor : MonoBehaviour
    {
        [SerializeField]
        [NotEmpty]
        [Order(-998)]
        private string section = string.Empty;

        [SerializeField]
        [NotEmpty]
        [Order(-997)]
        private string guid = Guid.NewGuid().ToString();

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(999)]
        private bool autoRemove = true;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected virtual void Awake()
        {
            SettingsSystem.OnBeforeSaveCallback += OnSaveBuffer;
        }

        /// <summary>
        /// Called when the script instance is being enabled
        /// </summary>
        protected virtual void Start()
        {
            if (SettingsSystem.TryGetValue(section, guid, out object item))
            {
                OnLoad(item);
                if (autoRemove)
                {
                    SettingsSystem.RemoveValue(section, guid);
                }
            }
            else
            {
                OnLoad(GetDefaultValue());
            }
        }

        /// <summary>
        /// Called when the script instance is being destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            SettingsSystem.OnBeforeSaveCallback -= OnSaveBuffer;
        }

        #region [Abstract methods]
        /// <summary>
        /// <br>Called when the settings manager saves the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Write</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to save processor value.</br>
        /// </summary>
        protected abstract object OnSave();

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Read</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected abstract void OnLoad(object value);

        /// <summary>
        /// <br>Called when settings file is not found or 
        /// target processor GUID is not found in loaded buffer.</br>
        /// <br>Implement this method to determine default value for this processor.</br>
        /// </summary>
        /// <returns>Default value of processor.</returns>
        public abstract object GetDefaultValue();
        #endregion

        #region [Save Event Wrapper]
        private void OnSaveBuffer(string section)
        {
            if(this.section == section)
            {
                SettingsSystem.AddValue(section, guid, OnSave());
            }
        }
        #endregion

        #region [Getter / Setter]
        public void SetGuid(string value)
        {
            guid = value;
        }

        public bool AutoRemove()
        {
            return autoRemove;
        }

        public void AutoRemove(bool value)
        {
            autoRemove = value;
        }
        #endregion
    }
}
