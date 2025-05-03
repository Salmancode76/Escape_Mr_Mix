/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine.AI;
using AuroraFPSRuntime.CoreModules.Coroutines;

namespace AuroraFPSRuntime.AIModules
{
    /// <summary>
    /// Custom IEnumerator yield instruction for AI NavMeshAgent.
    /// Suspends the coroutine execution until the AI reaches it's destination.
    /// </summary>
    public sealed class WaitForReach : AuroraYieldInstruction
    {
        // Base instruction properties.
        private NavMeshAgent navMeshAgent;

        /// <summary>
        /// WaitForReach constructor.
        /// </summary>
        public WaitForReach(NavMeshAgent navMeshAgent)
        {
            this.navMeshAgent = navMeshAgent;
        }

        protected override bool Update()
        {
            return !(!navMeshAgent.pathPending && (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) && !navMeshAgent.hasPath);
        }
    }
}