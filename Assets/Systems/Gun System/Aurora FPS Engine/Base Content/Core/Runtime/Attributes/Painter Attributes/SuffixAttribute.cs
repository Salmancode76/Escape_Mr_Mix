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
    public sealed class SuffixAttribute : PainterAttribute
    {
        public readonly string label;
        public readonly bool muted;

        public SuffixAttribute(string label)
        {
            this.label = label;
            this.muted = false;
            ItalicText = false;
        }

        public SuffixAttribute(string label, bool muted) : this(label)
        {
            this.muted = muted;
        }

        #region [Optional Options]
        public bool ItalicText { get; set; }
        #endregion
    }
}