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
    public sealed class ButtonAttribute : ApexBaseAttribute
    {
        public readonly string lable;
        public readonly string style;
        public readonly float height;

        public ButtonAttribute()
        {
            this.lable = "";
            this.style = "Button";
            this.height = 20;
        }

        public ButtonAttribute(string lable) : this()
        {
            this.lable = lable;
        }

        public ButtonAttribute(string lable, string style) : this(lable)
        {
            this.style = style;
        }

        public ButtonAttribute(string lable, string style, float height) : this(lable, style)
        {
            this.height = height;
        }
    }
}