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
    public sealed class FoldoutAttribute : LayoutAttribute
    {
        public FoldoutAttribute(string name) : base(name)
        {
            Style = null;
        }

        #region [Optional Options]
        public string Style { get; set; }
        #endregion
    }
}