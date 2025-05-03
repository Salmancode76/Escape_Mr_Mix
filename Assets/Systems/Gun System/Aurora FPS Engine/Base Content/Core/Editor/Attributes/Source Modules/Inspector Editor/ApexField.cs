/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public abstract class ApexField
    {
        public abstract void DrawFieldLayout();

        public abstract void DrawField(Rect position);

        public abstract float GetFieldHeight();

        public virtual bool IsVisible()
        {
            return true;
        }
    }
}