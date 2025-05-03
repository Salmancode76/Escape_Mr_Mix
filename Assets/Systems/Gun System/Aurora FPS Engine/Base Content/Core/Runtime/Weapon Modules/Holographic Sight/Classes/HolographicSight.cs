/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.UIModules.UIElements.Animation;
using UnityEngine;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using AuroraFPSRuntime.SystemModules.CameraSystems;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Sight/Holographic Sight")]
    [System.Obsolete]
    public sealed class HolographicSight : MonoBehaviour
    {
        [SerializeField]
        private Transition transition;

        // Stored required components.
        private PlayerController controller;

        private void Awake()
        {
            controller = transition.GetComponentInParent<PlayerController>();
        }

        private void OnEnable()
        {
            if (controller != null)
            {
                PlayerCamera cameraControl = controller.GetPlayerCamera();
                cameraControl.OnStartZoomCallback += transition.FadeIn;
                cameraControl.OnStopZoomCallback += transition.FadeOut;
            }
        }

        private void OnDisable()
        {
            if (controller != null)
            {
                PlayerCamera cameraControl = controller.GetPlayerCamera();
                cameraControl.OnStartZoomCallback -= transition.FadeIn;
                cameraControl.OnStopZoomCallback -= transition.FadeOut;
                transition.FadeOut();
            }
        }
    }
}