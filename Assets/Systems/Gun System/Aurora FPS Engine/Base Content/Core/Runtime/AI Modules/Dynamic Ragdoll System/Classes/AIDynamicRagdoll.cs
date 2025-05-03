/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/Dynamic Ragdoll/AI Dynamic Ragdoll")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AIDynamicRagdoll : DynamicRagdoll
    {
        /// <summary>
        /// Override this method to return animator component of the ragdoll character.
        /// Use GetComponent<Animator>() method.
        /// </summary>
        /// <param name="animator">Animator component of the ragdoll character.</param>
        protected override void CopyAnimator(out Animator animator)
        {
            animator = GetComponent<Animator>();
        }
    }
}