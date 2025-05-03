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
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [CreateAssetMenu(fileName = "New Bullet Item", menuName ="Aurora FPS Engine/Weapon/Bullet/Standard Bullet", order = 100)]
    [ComponentIcon("Bullet")]
    public class BulletItem : BaseItem, IBulletItem
    {
        [SerializeField]
        [Foldout("Bullet Settings", Style = "Header")]
        [MinValue(0)]
        [Order(50)]
        private float damage;

        [SerializeField]
        [Foldout("Bullet Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(51)]
        private float impactImpulse;

        [SerializeField]
        [Foldout("Bullet Settings", Style = "Header")]
        [NotNull]
        [Order(99)]
        private DecalMapping decalMapping;

        #region [Getter / Setter]
        public float GetDamage()
        {
            return damage * WeaponUtilities.DamageMultiplier;
        }

        public void SetDamage(float value)
        {
            damage = value;
        }

        public float GetImpactImpulse()
        {
            return impactImpulse;
        }

        public void SetImpactImpulse(float value)
        {
            impactImpulse = value;
        }

        public DecalMapping GetDecalMapping()
        {
            return decalMapping;
        }

        public void SetDecalMapping(DecalMapping value)
        {
            decalMapping = value;
        }
        #endregion
    }
}