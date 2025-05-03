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
//    [ConditionMenu("On Find Target", "Field Of View/On Find Target", Description = "Called when any target is in view.")]
//    [RequireComponent(typeof(AIFieldOfView))]
//    public class OnFindTargetCondition : Condition
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

//            fieldOfView.OnFindTargetsCallback += () => isExecuted = true;

//            fieldOfView.OnLostTargetsCallback += () => isExecuted = false;
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