/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.InventoryModules;
using AuroraFPSRuntime.WeaponModules;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Looting/Loot Weapon")]
    [DisallowMultipleComponent]
    public sealed class LootWeapon : LootObject
    {
        [SerializeField]
        [NotNull]
        private EquippableItem item;

        /// <summary>
        /// Called when character being loot object.
        /// </summary>
        /// <param name="other">Reference of loot object transform.</param>
        /// <returns>The success of looting the specified object.</returns>
        protected override bool OnLoot(Transform other)
        {
            int messageCode = GetMessageCode();
            WeaponInventorySystem inventorySystem = other.GetComponent<WeaponInventorySystem>();
            if (inventorySystem != null)
            {
                switch (messageCode)
                {
                    case 1:
                        if (inventorySystem.AddItem(item))
                        {
                            inventorySystem.UseItem(item);
                            return true;
                        }
                        return false;
                    case 2:
                        if (inventorySystem.SwapItem(item))
                        {
                            return true;
                        }
                        return false;
                }
            }
            return false;
        }

        protected override void CalculateMessageCode(Transform other, out int messageCode)
        {
            messageCode = 0;
            WeaponInventorySystem inventorySystem = other.GetComponent<WeaponInventorySystem>();
            if (inventorySystem != null)
            {
                if (inventorySystem.HasSpace(item.GetItemType()))
                {
                    messageCode = 1;
                }
                else
                {
                    messageCode = 2;
                }
            }
        }
    }
}