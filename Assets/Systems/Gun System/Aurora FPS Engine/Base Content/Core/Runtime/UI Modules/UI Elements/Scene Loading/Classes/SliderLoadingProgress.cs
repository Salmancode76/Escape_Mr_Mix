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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Scene Loading/Slider Loading Progress")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public sealed class SliderLoadingProgress : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private SceneLoader sceneLoader;

        // Stored required components.
        private Slider slider;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            slider.value = sceneLoader.GetLoadingProgress();
        }
    }
}
