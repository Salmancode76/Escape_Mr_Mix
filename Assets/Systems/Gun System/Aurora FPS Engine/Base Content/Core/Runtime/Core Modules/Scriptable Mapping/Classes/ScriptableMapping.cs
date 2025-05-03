/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.CoreModules
{
    /// <summary>
    /// Represents base class for all scriptable mapping.
    /// </summary>
    public abstract class ScriptableMapping : ScriptableObject, IScriptableMapping
    {
        /// <summary>
        /// Get mapping length.
        /// </summary>
        public abstract int GetMappingLength();
    }
}