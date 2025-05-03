/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Scene Loading/Value Loading Progress")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class ValueLoadingProgress : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private SceneLoader sceneLoader;

        [SerializeField]
        private string format = "F0";

        // Stored required components.
        private Text text;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            text = GetComponent<Text>();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            text.text = (sceneLoader.GetLoadingProgress() * 100).ToString(format);
        }
    }
}
