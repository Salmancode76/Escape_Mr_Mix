/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HealthFunctionMenu : Attribute
    {
        public readonly string name;
        public readonly string path;

        public HealthFunctionMenu(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
    }
}