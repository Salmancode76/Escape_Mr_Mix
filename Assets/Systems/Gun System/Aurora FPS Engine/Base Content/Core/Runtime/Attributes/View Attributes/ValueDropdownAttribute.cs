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
    public sealed class ValueDropdownAttribute : ViewAttribute
    {
        public readonly string ienumerable;

        public ValueDropdownAttribute(string ienumerable)
        {
            this.ienumerable = ienumerable;
        }
    }
}