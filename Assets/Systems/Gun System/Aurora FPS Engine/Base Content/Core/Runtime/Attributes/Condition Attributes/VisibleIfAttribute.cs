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
    public sealed class VisibleIfAttribute : ConditionAttribute
    {
        public VisibleIfAttribute(string propertyName) : base(propertyName)
        {
        }

        public VisibleIfAttribute(string propertyName, bool condition) : base(propertyName, condition)
        {
        }

        public VisibleIfAttribute(string enumProperty, string enumValue) : base(enumProperty, enumValue)
        {
        }

        public VisibleIfAttribute(string firstProperty, string condition, string secondProperty) : base(firstProperty, condition, secondProperty)
        {
        }
    }
}