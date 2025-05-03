///* ================================================================
//   ----------------------------------------------------------------
//   Project   :   Aurora FPS Engine
//   Publisher :   Infinite Dawn
//   Developer :   Tamerlan Shakirov
//   ----------------------------------------------------------------
//   Copyright © 2017 Tamerlan Shakirov All rights reserved.
//   ================================================================ */

//using AuroraFPSRuntime.SystemModules.HealthModules;
//using UnityEngine;

//namespace AuroraFPSRuntime.AIModules.Conditions
//{
//    [RequireComponent(typeof(AIFieldOfView))]
//    [ConditionMenu("On Kill Targets", "Health/On Kill Targets")]
//    public class OnKillTargetsCondition : Condition
//    {
//        // Stored required components.
//        private AIFieldOfView fieldOfView;

//        /// <summary>
//        /// Initializing required properties.
//        /// </summary>
//        /// <param name="core">Target AIController instance.</param>
//        protected override void OnInitialize(AIController core)
//        {
//            fieldOfView = core.GetComponent<AIFieldOfView>();
//        }

//        /// <summary>
//        /// Condition for translate to the next AI behaviour.
//        /// </summary>
//        public override bool IsExecuted()
//        {
//            for (int i = 0; i < fieldOfView.GetVisibleTargetCount(); i++)
//            {
//                IHealth health = fieldOfView.GetVisibleTarget(i).GetComponent<IHealth>();
//                if(health != null && health.IsAlive())
//                {
//                    return false;
//                }
//            }
//            return true;
//        }
//    }
//}