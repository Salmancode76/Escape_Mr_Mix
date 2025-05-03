/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PrefixAttribute : PainterAttribute
    {
        public readonly string label;
        public readonly bool beforeProperty;

        public PrefixAttribute(string label)
        {
            this.label = label;
            Style = "Label";
        }

        public PrefixAttribute(string label, bool beforeProperty) : this(label)
        {
            this.beforeProperty = beforeProperty;
        }

        #region [Getter / Setter]
        public string Style { get; set; }
        #endregion
    }
}