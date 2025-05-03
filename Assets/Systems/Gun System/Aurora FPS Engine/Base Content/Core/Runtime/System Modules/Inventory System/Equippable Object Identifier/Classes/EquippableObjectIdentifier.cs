/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Inventory System/Equippable Object Identifier")]
    [DisallowMultipleComponent]
    public sealed class EquippableObjectIdentifier : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private EquippableItem item;

        #region [Getter / Setter]
        public EquippableItem GetItem()
        {
            return item;
        }

        public void SetItem(EquippableItem value)
        {
            item = value;
        }
        #endregion
    }
}