/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.AIModules.Conditions
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConditionMenuAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Path;

        public ConditionMenuAttribute(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        #region [Optional]
        public string Description { get; set; }
        #endregion
    }
}

