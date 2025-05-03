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
    public sealed class PropertySpaceAttribute : PainterAttribute
    {
        public readonly float space;

        public PropertySpaceAttribute()
        {
            this.space = 15.0f;
        }

        public PropertySpaceAttribute(float space)
        {
            this.space = space;
        }
    }
}