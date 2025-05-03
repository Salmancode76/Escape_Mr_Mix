/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Conditions
{
    [RequireComponent(typeof(AICharacterHealth))]
    [ConditionMenu("On Take Damage(Int32 amount)", "Health/On Take Damage(Int32 amount)", Description = "Called every time when AI take specific(value) damage.")]
    public class OnTakeDamageIntCondition : OnTakeDamageCondition
    {
        [SerializeField] private int value;

        /// <summary>
        /// Register health take damage callback function.
        /// </summary>
        protected override void RegisterCallbackFunction()
        {
            health.OnTakeDamageCallback += value =>
            {
                InvokeDefaultCallback();
            };
        }
    }
}