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
    public sealed class DropdownReferenceAttribute : ViewAttribute
    {
        public DropdownReferenceAttribute()
        {
            FoldoutToggle = true;
        }


        #region [Optional Options]
        public bool FoldoutToggle { get; set; }
        #endregion
    }
}