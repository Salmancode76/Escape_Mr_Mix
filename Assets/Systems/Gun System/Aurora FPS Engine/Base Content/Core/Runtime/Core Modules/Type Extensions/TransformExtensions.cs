/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.TypeExtensions
{
    public static class TransformExtensions
    {
        public static bool SendDamage(this Component component, int amount)
        {
            IDamageable damageable = component.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(amount);
                return true;
            }
            return false;
        }

        public static bool SendDamage(this RaycastHit hitInfo, int amount)
        {
            IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(amount);
                return true;
            }
            return false;
        }
    }
}

