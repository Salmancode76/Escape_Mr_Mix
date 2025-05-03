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

namespace AuroraFPSRuntime
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Animator Utilities/Animation Event Disabler")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AnimatorEventDisabler : MonoBehaviour
    {
        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Animator animator = GetComponent<Animator>();
            animator.fireEvents = false;
        }
    }
}
