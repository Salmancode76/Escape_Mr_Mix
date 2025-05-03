/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [System.Serializable]
    public class RecoilDictionary : SerializableDictionary<ControllerState, RecoilPattern>
    {
        [SerializeField]
        private ControllerState[] keys;

        [SerializeField]
        private RecoilPattern[] values;

        protected override ControllerState[] GetKeys()
        {
            return keys;
        }

        protected override RecoilPattern[] GetValues()
        {
            return values;
        }

        protected override void SetKeys(ControllerState[] keys)
        {
            this.keys = keys;
        }

        protected override void SetValues(RecoilPattern[] values)
        {
            this.values = values;
        }
    }

    [CreateAssetMenu(fileName = "Recoil Mapping", menuName = "Aurora FPS Engine/Weapon/Recoil Mapping", order = 130)]
    public class RecoilMapping : ScriptableMappingDictionary<RecoilDictionary, ControllerState, RecoilPattern> { }
}