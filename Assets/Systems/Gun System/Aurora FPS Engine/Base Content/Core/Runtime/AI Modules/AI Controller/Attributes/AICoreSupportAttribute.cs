/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.AIModules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class AICoreSupportAttribute : Attribute
    {
        public readonly Type target;

        public AICoreSupportAttribute(Type target)
        {
            this.target = target;
        }
    }
}