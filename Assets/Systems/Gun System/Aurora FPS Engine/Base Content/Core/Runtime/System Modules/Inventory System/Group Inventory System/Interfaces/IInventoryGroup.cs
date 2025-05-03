/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.InventoryModules.GroupInventory
{
    public interface IInventoryGroup
    {
        /// <summary>
        /// Add new slot in inventory.
        /// </summary>
        /// <param name="input">Input key.</param>
        /// <param name="item">Inventory item.</param>
        /// <returns></returns>
        void AddSlot(string input, InventoryItem item);

        /// <summary>
        /// Remove slot from inventory.
        /// </summary>
        /// <param name="input">Input key.</param>
        bool RemoveSlot(string input);
    }
}