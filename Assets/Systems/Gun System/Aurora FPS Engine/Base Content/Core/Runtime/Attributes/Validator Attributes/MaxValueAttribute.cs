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
    public class MaxValueAttribute : ValidatorAttribute
    {
        public readonly float value;
        public readonly string property;

        public MaxValueAttribute(float value)
        {
            this.value = value;
        }

        public MaxValueAttribute(string property)
        {
            this.property = property;
            this.value = 100;
        }
    }
}