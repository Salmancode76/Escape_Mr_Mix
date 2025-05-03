/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Internal.Integrations
{
    public abstract class IntegrationEditor
    {
        /// <summary>
        /// Called for rendering and handling GUI events.
        /// </summary>
        public abstract void OnGUI(Rect position);

        /// <summary>
        /// Total height required to draw this integration.
        /// </summary>
        public virtual float GetHeight()
        {
            return 20;
        }

        /// <summary>
        /// <br>Make a single press button of the right corner.</br>
        /// <br>The user clicks them and something happens immediately.</br>
        /// </summary>
        /// <param name="position">
        /// <br>Rectangle on the screen to use for the button.</br>
        /// <br>After drawing the button, the position width will be reduced relative the width of the button.</br>
        /// </param>
        /// <param name="label">Text to display on the button.</param>
        /// <param name="width">
        /// <br>Custom button width.</br>
        /// <br>By default calculated automatically by label.</br>
        /// </param>
        protected bool RightButton(ref Rect position, string label, float? width = null)
        {
            GUIContent nameContent = new GUIContent(label);
            GUIStyle style = EditorStyles.toolbarButton;
            style.fixedHeight = position.height;
            if(width == null)
            {
                width = style.CalcSize(nameContent).x;
            }
            Rect _pos = new Rect(position.xMax - width.Value + 1, position.y, width.Value, position.height);
            position.width -= width.Value;
            return GUI.Button(_pos, nameContent, style);
        }
    }
}