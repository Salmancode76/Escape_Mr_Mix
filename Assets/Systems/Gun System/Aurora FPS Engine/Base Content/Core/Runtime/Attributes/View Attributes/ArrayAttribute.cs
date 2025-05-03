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
    public sealed class ArrayAttribute : ViewAttribute
    {
        public const string DefaultElementLabelFormat = "Element {niceIndex}";
        public const string DefaultCountLabelFormat = "{count} Elements";

        /// <summary>
        /// Apex array attribute.
        /// </summary>
        public ArrayAttribute()
        {
            ElementLabel = DefaultElementLabelFormat;
            CountLabel = DefaultCountLabelFormat;
        }

        #region [Optional Parameters]
        /// <summary>
        /// Custom element name display format. Arguments: {index}, {niceIndex}
        /// </summary>
        public string ElementLabel { get; set; }

        /// <summary>
        /// Custom element count display format. Arguments: {count}
        /// </summary>
        public string CountLabel { get; set; }

        public string GetElementLabelCallback { get; set; }
        #endregion
    }
}