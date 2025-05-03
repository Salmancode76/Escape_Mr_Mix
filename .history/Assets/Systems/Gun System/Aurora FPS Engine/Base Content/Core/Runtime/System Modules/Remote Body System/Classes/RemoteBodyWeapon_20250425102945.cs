/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.SystemModules.InventoryModules;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Remote Body/Remote Body Weapon")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class RemoteBodyWeapon : MonoBehaviour
    {
        [System.Serializable]
        private sealed class WeaponSettings
        {
            [SerializeField]
            [NotNull]
            private GameObject weapon;

            [SerializeField]
            private AnimatorState selectState = string.Empty;

            [SerializeField]
            private Transform leftHandTarget;

            [SerializeField]
            private Transform rightHandTarget;

            #region [Getter / Setter]
            public GameObject GetWeapon()
            {
                return weapon;
            }

            public void SetWeapon(GameObject value)
            {
                weapon = value;
            }

            public AnimatorState GetAnimatorState()
            {
                return selectState;
            }

            public void SetAnimatorState(AnimatorState value)
            {
                selectState = value;
            }

            public Transform GetLeftHandTarget()
            {
                return leftHandTarget;
            }

            public void SetLeftHandTarget(Transform value)
            {
                leftHandTarget = value;
            }

            public Transform GetRightHandTarget()
            {
                return rightHandTarget;
            }

            public void SetRightHandTarget(Transform value)
            {
                rightHandTarget = value;
            }
            #endregion
        }

        [System.Serializable]
        private sealed class WeaponDictionary : SerializableDictionary<EquippableItem, WeaponSettings>
        {
            [SerializeField]
            private EquippableItem[] keys;

            [SerializeField]
            private WeaponSettings[] values;

            protected override EquippableItem[] GetKeys()
            {
                return keys;
            }

            protected override WeaponSettings[] GetValues()
            {
                return values;
            }

            protected override void SetKeys(EquippableItem[] keys)
            {
                this.keys = keys;
            }

            protected override void SetValues(WeaponSettings[] values)
            {
                this.values = values;
            }
        }

        [SerializeField]
        [NotNull]
        private InventorySystem inventorySystem;

        [SerializeField]
        private WeaponDictionary weaponDictionary;

        // Stored required components.
        private Animator animator;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Debug.Assert(inventorySystem != null, $"<b><color=#FF0000>Attach reference of the inventory system to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Inventory System<i>(field)</i>.</color></b>");
            animator = GetComponent<Animator>();
            DiactivateAllWeapons();
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            inventorySystem.OnEquipStartedCallback += AcivateWeapon;
            inventorySystem.OnTossItemCallback += DiacivateWeapon;
        }

        /// <summary>
        /// Called by the Animator Component immediately before it updates its internal IK system.
        /// </summary>
        /// <param name="layerIndex">The index of the layer on which the IK solver is called.</param>
        private void OnAnimatorIK(int layerIndex)
        {
            Transform equippedTrasform = inventorySystem.GetEquippedTransform();
            EquippableItem equippableItem = inventorySystem.GetEquippedItem();
            if (equippedTrasform != null && equippableItem != null)
            {
                if (weaponDictionary.TryGetValue(equippableItem, out WeaponSettings settings))
                {

                    animator.SetIKPosition(AvatarIKGoal.LeftHand, settings.GetLeftHandTarget().position);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, settings.GetLeftHandTarget().rotation);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

                    animator.SetIKPosition(AvatarIKGoal.RightHand, settings.GetRightHandTarget().position);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, settings.GetRightHandTarget().rotation);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                }
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            inventorySystem.OnEquipStartedCallback -= AcivateWeapon;
            inventorySystem.OnTossItemCallback -= DiacivateWeapon;
        }

        public void AcivateWeapon(EquippableItem equippableItem)
        {
            DiactivateAllWeapons();

            if (weaponDictionary.TryGetValue(equippableItem, out WeaponSettings settings))
            {
                settings.GetWeapon().SetActive(true);
                if (!string.IsNullOrEmpty(settings.GetAnimatorState()))
                {
                    animator.CrossFadeInFixedTime(settings.GetAnimatorState());
                }
            }
        }

        public void DiactivateAllWeapons()
        {
            foreach (KeyValuePair<EquippableItem, WeaponSettings> item in weaponDictionary)
            {
                item.Value.GetWeapon().SetActive(false);
            }
        }

        private void DiacivateWeapon(InventoryItem inventoryItem)
        {
            if (inventoryItem is EquippableItem equippableItem)
            {
                if (weaponDictionary.TryGetValue(equippableItem, out WeaponSettings settings))
                {
                    settings.GetWeapon().SetActive(false);
                }
            }
        }
    }
}