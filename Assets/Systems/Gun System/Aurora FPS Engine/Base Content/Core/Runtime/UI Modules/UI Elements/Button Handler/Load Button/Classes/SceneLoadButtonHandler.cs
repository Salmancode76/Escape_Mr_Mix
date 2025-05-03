/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.UI;
using AuroraFPSRuntime.CoreModules.SceneManagement;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Button Handler/Scene Load Button Handler")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class SceneLoadButtonHandler : MonoBehaviour
    {
        private enum InteractiveType
        {
            None,
            Hide,
            Disable
        }

        [SerializeField]
        private InteractiveType interactiveType = InteractiveType.Disable;

        // Stored required components.
        private Button button;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            button = GetComponent<Button>();
        }

        private void LateUpdate()
        {
            int index = SceneManager.GetTargetScene();
            switch (interactiveType)
            {
                case InteractiveType.Hide:
                    gameObject.SetActive(index >= 0);
                    break;
                case InteractiveType.Disable:
                    button.interactable = index >= 0;
                    break;
            }
        }
    }
}
