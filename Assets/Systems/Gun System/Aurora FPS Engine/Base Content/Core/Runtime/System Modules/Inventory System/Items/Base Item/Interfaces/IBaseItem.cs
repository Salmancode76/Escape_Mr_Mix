/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    public interface IBaseItem
    {
        /// <summary>
        /// Unique name of the item.
        /// </summary>
        /// <returns>Item name.</returns>
        string GetItemName();
    }
}