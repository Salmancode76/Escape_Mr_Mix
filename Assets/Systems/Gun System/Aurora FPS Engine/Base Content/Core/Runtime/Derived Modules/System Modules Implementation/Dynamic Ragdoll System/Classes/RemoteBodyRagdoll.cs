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

namespace AuroraFPSRuntime
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Remote Body/Remote Body Ragdoll")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class RemoteBodyRagdoll : DynamicRagdoll
    {
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            OnBlendCompleteCallback += FixCharacterPosition;
        }

        /// <summary>
        /// Override this method to initialize animator component of the ragdoll character.
        /// Use GetComponent<Animator>() method.
        /// </summary>
        /// <param name="animator">Animator component of the ragdoll character.</param>
        protected override void CopyAnimator(out Animator animator)
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Override this method to initialize transform component of the ragdoll character.
        /// </summary>
        /// <param name="transform">Transform component of the ragdoll character.</param>
        protected override void CopyRagdollTransform(out Transform transform)
        {
            transform = this.transform;
        }

        /// <summary>
        /// Calculate body direction, when character get up.
        /// </summary>
        /// <returns>Body direction when character get up.</returns>
        protected override Vector3 CalculateBodyDirection()
        {
            return transform.forward;
        }

        /// <summary>
        /// Fix character position when character blended to animator.
        /// </summary>
        public virtual void FixCharacterPosition()
        {
            if (StateIs(RagdollState.Animated))
                transform.position = transform.root.position;
        }
    }
}