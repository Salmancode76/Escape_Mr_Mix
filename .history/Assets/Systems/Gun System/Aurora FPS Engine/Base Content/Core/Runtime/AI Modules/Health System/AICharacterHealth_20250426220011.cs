using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;
using UnityEngine.AI;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/Health/AI Health")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AIController))]
    public sealed class AICharacterHealth : CharacterHealth
    {
        // Stored required components.
        private AIController controller;
        private NavMeshAgent navMeshAgent;

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<AIController>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        protected override void OnRevive()
        {
            base.OnRevive();
            
            try
            {
                // Only sleep if the agent is on a NavMesh
                if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
                {
                    controller.Sleep(false);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to wake AI controller: " + e.Message);
            }
        }

        protected override void OnDead()
        {
            base.OnDead();
            
            try
            {
                // Only sleep if the agent is on a NavMesh
                if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
                {
                    controller.Sleep(true);
                }
                else
                {
                    // Alternative approach when NavMesh agent isn't valid
                    if (navMeshAgent != null)
                    {
                        navMeshAgent.enabled = false;
                    }
                    
                    // Handle any other necessary death logic that doesn't involve NavMesh
                    // For example, disable AI scripts, animations, etc.
                    var aiComponents = GetComponents<MonoBehaviour>();
                    foreach (var component in aiComponents)
                    {
                        // Skip health-related components to avoid breaking the death process
                        if (component is CharacterHealth || component == this)
                            continue;
                            
                        // Disable AI behavior components
                        if (component is AIController || component.GetType().Namespace.Contains("AIModules"))
                        {
                            component.enabled = false;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to sleep AI controller: " + e.Message);
            }
        }
    }
}