///* ================================================================
//   ----------------------------------------------------------------
//   Project   :   Aurora FPS Engine
//   Publisher :   Infinite Dawn
//   Developer :   Tamerlan Shakirov
//   ----------------------------------------------------------------
//   Copyright © 2017 Tamerlan Shakirov All rights reserved.
//   ================================================================ */

//using UnityEngine;

//namespace AuroraFPSRuntime.AIModules.Conditions
//{
//    [RequireComponent(typeof(AIFieldOfView))]
//    [ConditionMenu("On Lost Target", "Field Of View/On Lost Target", Description = "Called when the AI loses all targets from view.")]
//    public class OnLostTargetCondition : Condition
//    {
//        private bool isExecuted;
//        private AIFieldOfView fieldOfView;

//        /// <summary>
//        /// Initializing required properties.
//        /// </summary>
//        /// <param name="core">Target AIController instance.</param>
//        protected override void OnInitialize(AIController core)
//        {
//            fieldOfView = core.GetComponent<AIFieldOfView>();

//            fieldOfView.OnLostTargetsCallback += () => isExecuted = true;

//            fieldOfView.OnFindTargetsCallback += () => isExecuted = false;
//        }

//        /// <summary>
//        /// Condition for translate to the next AI behaviour.
//        /// </summary>
//        public override bool IsExecuted()
//        {
//            return isExecuted;
//        }
//    }
//}