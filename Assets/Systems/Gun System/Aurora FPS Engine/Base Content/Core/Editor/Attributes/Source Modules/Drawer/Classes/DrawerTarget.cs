/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DrawerTarget : Attribute
    {
        public readonly Type target;

        public DrawerTarget(Type target)
        {
            this.target = target;
            SubClasses = false;
        }

        #region [Optional Parameters]
        public bool SubClasses { get; set; }
        #endregion
    }
}