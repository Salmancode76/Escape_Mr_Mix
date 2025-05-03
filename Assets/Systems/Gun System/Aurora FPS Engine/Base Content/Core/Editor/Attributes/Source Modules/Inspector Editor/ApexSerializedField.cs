/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEditor;

namespace AuroraFPSEditor.Attributes
{
    public abstract class ApexSerializedField : ApexField
    {
        public readonly SerializedProperty TargetSerializedProperty;
        public readonly int Order;

        public ApexSerializedField(SerializedProperty source)
        {
            this.TargetSerializedProperty = source;
            Order = -1;
        }

        public ApexSerializedField(SerializedProperty source, int order) : this(source)
        {
            this.Order = order;
        }
    }
}