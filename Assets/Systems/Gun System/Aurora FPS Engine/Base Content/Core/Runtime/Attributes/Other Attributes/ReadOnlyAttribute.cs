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
    [Flags]
    public enum EditorState
    {
        Editor,
        Play,
        Pause,
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ReadOnlyAttribute : ApexBaseAttribute
    {
        public readonly EditorState state;

        public ReadOnlyAttribute()
        {
            state = EditorState.Editor | EditorState.Play | EditorState.Pause;
        }

        public ReadOnlyAttribute(EditorState state)
        {
            this.state = state;
        }
    }
}