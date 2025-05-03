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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Scene Loading/Circle Loading Progress")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class CircleLoadingProgress : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private SceneLoader sceneLoader;

        // Stored required components.
        private Image image;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            image = GetComponent<Image>();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            image.fillAmount = sceneLoader.GetLoadingProgress();
        }

        #region[Editor Section]
#if UNITY_EDITOR
        [Button("Auto Setup Image")]
        private void AutoSetupImage()
        {
            image = GetComponent<Image>();
            if(image != null)
            {
                image.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Aurora FPS Engine/Base Content/UI/Sprites/Crosshair Sprites/Shotgun Crosshair Element.png");
                if(image.sprite == null)
                {
                    image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                }
                image.maskable = false;
                image.raycastTarget = false;
                image.type = Image.Type.Filled;
                image.fillMethod = Image.FillMethod.Radial360;
                image.fillOrigin = 2;
                image.fillClockwise = true;
                image.fillAmount = 0.0f;
            }
        }
#endif
        #endregion
    }
}
