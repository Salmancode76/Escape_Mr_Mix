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
    public interface IPropertyPositionModifyReceiver
    {
        /// <summary>
        /// Called before drawing target property, for changing property Rectangle position.
        /// </summary>
        /// <param name="position">Target property position.</param>
        void ModifyPropertyPosition(ref Rect position);
    }
}