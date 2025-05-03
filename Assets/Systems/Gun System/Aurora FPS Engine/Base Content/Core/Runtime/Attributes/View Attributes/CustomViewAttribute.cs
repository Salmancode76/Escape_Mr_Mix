/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.Attributes
{
    public sealed class CustomViewAttribute : ViewAttribute
    {
        public string ViewInitialization { get; set; }
        public string ViewGUI { get; set; }
        public string ViewHeight { get; set; }
    }
}