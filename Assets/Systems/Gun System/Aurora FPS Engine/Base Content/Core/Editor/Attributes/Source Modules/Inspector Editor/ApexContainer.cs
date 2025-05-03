/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEditor;

namespace AuroraFPSEditor.Attributes
{
    public class ApexContainer : ApexProperty
    {
        private Padding padding;

        public ApexContainer(SerializedProperty target, Padding padding) : base(target)
        {
            this.padding = padding;
        }

        #region [Getter / Setter]
        public Padding GetPadding()
        {
            return padding;
        }

        public void SetPadding(Padding value)
        {
            padding = value;
        }
        #endregion
    }
}