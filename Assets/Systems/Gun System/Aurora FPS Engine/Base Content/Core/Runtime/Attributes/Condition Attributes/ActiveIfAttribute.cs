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
    public sealed class ActiveIfAttribute : ConditionAttribute
    {
        public ActiveIfAttribute(string propertyName) : base(propertyName)
        {
        }

        public ActiveIfAttribute(string propertyName, bool condition) : base(propertyName, condition)
        {
        }

        public ActiveIfAttribute(string firstProperty, string condition, string secondProperty) : base(firstProperty, condition, secondProperty)
        {
        }
    }
}