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
    public interface IInventorySystem<T>
    {
        /// <summary>
        /// Add new item in inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        bool AddItem(T item);

        /// <summary>
        /// Remove item from inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        bool RemoveItem(T item);
    }
}