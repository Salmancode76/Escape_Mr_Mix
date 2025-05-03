/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Sight/Sniper Sight")]
    [DisallowMultipleComponent]
    public sealed class SniperSight : MonoBehaviour
    {
        [SerializeField]
        [ReorderableList(ElementLabel = null)]
        private MeshRenderer[] sightComponents;

        [SerializeField]
        [ReorderableList(ElementLabel = null)]
        private MeshRenderer[] plugComponents;

        // Stored required properties.
        private PlayerController controller;

        private void Awake()
        {
            controller = GetComponentInParent<PlayerController>();
            if(controller != null)
            {
                controller.GetPlayerCamera().OnFOVProgressCallback += OnFOVChanged;
            }
        }

        private void OnDestroy()
        {
            if (controller != null)
            {
                controller.GetPlayerCamera().OnFOVProgressCallback -= OnFOVChanged;
            }
        }

        private void OnFOVChanged(float progress)
        {
            PlayerCamera cameraControl = controller.GetPlayerCamera();
            for (int i = 0; i < sightComponents.Length; i++)
            {
                MeshRenderer meshRenderer = sightComponents[i];
                if(meshRenderer != null)
                {
                    Material material = meshRenderer.material;
                    Color targetColor = material.color;
                    targetColor.a = cameraControl.IsZooming() ? 1.0f : 0.0f;
                    material.color = Color.Lerp(material.color, targetColor, progress);
                }
            }

            for (int i = 0; i < plugComponents.Length; i++)
            {
                MeshRenderer meshRenderer = plugComponents[i];
                if (meshRenderer != null)
                {
                    Material material = meshRenderer.material;
                    Color targetColor = material.color;
                    targetColor.a = cameraControl.IsZooming() ? 0.0f : 1.0f;
                    material.color = Color.Lerp(material.color, targetColor, progress);
                }
            }
        }
    }
}