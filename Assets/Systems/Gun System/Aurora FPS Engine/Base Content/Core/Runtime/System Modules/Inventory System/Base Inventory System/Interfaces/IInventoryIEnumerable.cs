/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System.Collections.Generic;

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    public interface IInventoryIEnumerable<T>
    {
        /// <summary>
        /// Iterate through all inventory items in inventory.
        /// </summary>
        IEnumerable<T> Items { get; }
    }
}