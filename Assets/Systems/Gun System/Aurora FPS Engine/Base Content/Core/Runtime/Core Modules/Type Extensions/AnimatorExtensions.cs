/* ================================================================
 ----------------------------------------------------------------
 Project   :   Aurora FPS Engine
 Publisher :   Infinite Dawn
 Developer :   Tamerlan Shakirov
 ----------------------------------------------------------------
 Copyright © 2017 Tamerlan Shakirov All rights reserved.
 ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.TypeExtensions
{
    public static class AnimatorExtensions
    {
        public static float GetAnimationClipLength(this Animator animator, string name)
        {
            if (animator != null)
            {
                AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                for (int i = 0; i < clips.Length; i++)
                {
                    AnimationClip clip = clips[i];
                    if (clip != null && clip.name == name)
                    {
                        return clip.length;
                    }
                }
            }
            return 0.0f;
        }
    }
}