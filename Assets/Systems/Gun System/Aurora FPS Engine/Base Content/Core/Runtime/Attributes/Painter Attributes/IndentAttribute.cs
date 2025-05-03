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
    public sealed class IndentAttribute : PainterAttribute
    {
        public readonly int level;
        public readonly bool following;

        public IndentAttribute()
        {
            level = 1;
            following = false;
        }

        public IndentAttribute(int level)
        {
            this.level = level;
        }

        public IndentAttribute(int level, bool following) : this(level)
        {
            this.following = following;
        }
    }
}