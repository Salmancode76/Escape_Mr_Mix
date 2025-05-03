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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ReferenceContent : ApexBaseAttribute
    {
        public readonly string name;
        public readonly string path;

        public ReferenceContent(string name, string path)
        {
            this.name = name;
            this.path = path;

            Hided = false;
        }

        #region [Getter / Setter]
        public bool Hided { get; set; }
        #endregion
    }
}
