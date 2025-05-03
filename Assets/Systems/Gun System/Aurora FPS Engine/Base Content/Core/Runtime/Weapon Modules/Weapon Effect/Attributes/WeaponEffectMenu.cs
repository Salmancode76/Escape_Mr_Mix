/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.WeaponModules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class WeaponEffectMenu : Attribute
    {
        public readonly string Name;
        public readonly string Path;

        public WeaponEffectMenu(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}