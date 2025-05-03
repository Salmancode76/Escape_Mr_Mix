/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules
{
    public interface IEquippableAnimation
    {
        /// <summary>
        /// Play pull animation clip.
        /// </summary>
        void PlayPullAnimation();

        /// <summary>
        /// Play push animation clip.
        /// </summary>
        void PlayPushAnimation();
    }
}