/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.UIModules.IMGUI.Crosshair
{
    public interface ICrosshairPreset
    {
        /// <summary>
        /// Draw crosshair elements GUI layout taking into account hide state.
        /// </summary>
        /// <param name="spread"></param>
        void DrawElementsLayout(float spread);
    }
}