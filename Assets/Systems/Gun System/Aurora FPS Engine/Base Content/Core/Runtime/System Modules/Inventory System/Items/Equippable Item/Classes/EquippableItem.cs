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
    [CreateAssetMenu(fileName = "New Equippable Item", menuName = "Aurora FPS Engine/Inventory/Equippable Item", order = 91)]
    [ComponentIcon("Equippable Item")]
    public class EquippableItem : InventoryItem
    {
        [SerializeField]
        [NotNull]
        [Foldout("First Person Settings", Style = "Header")]
        private GameObject firstPersonObject;

        [SerializeField]
        [Foldout("First Person Settings", Style = "Header")]
        private Vector3 firstPersonPosition;

        [SerializeField]
        [Foldout("First Person Settings", Style = "Header")]
        private Vector3 firstPersonRotation;

        [SerializeField]
        [MinValue(0.0f)]
        [Foldout("Equip Settings", Style = "Header")]
        [Order(252)]
        private float selectTime;

        [SerializeField]
        [MinValue(0.0f)]
        [Foldout("Equip Settings", Style = "Header")]
        [Order(253)]
        private float hideTime;

        #region [Getter / Setter]
        public GameObject GetFirstPersonObject()
        {
            return firstPersonObject;
        }

        public void SetFirstPersonObject(GameObject value)
        {
            firstPersonObject = value;
        }

        public Vector3 GetFirstPersonPosition()
        {
            return firstPersonPosition;
        }

        public void SetFirstPersonPosition(Vector3 value)
        {
            firstPersonPosition = value;
        }

        public Vector3 GetFirstPersonRotation()
        {
            return firstPersonRotation;
        }

        public void SetFirstPersonRotation(Vector3 value)
        {
            firstPersonRotation = value;
        }

        public float GetSelectTime()
        {
            return selectTime;
        }

        public void SetSelectTime(float value)
        {
            selectTime = value;
        }

        public float GetHideTime()
        {
            return hideTime;
        }

        public void SetHideTime(float value)
        {
            hideTime = value;
        }
        #endregion
    }
}