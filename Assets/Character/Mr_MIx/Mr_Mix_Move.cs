using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Mr_Mix_Move : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRange = 10f;
    public float attackRange = 2f;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    private NavMeshAgent AIAgent;
    private int currentPatrolIndex = 0;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private bool hasSeenPlayer = false;

    void Start()
    {
        AIAgent = GetComponent<NavMeshAgent>();
        AIAgent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
            AIAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            hasSeenPlayer = true;
        }

        switch (currentState)
        {
            case State.Patrol:
                PatrolBehavior(distance);
                break;

            case State.Chase:
                ChaseBehavior(distance);
                break;

            case State.Attack:
                AttackBehavior(distance);
                break;
        }
    }

    void PatrolBehavior(float distance)
    {
        AIAgent.speed = patrolSpeed;

        if (hasSeenPlayer)
        {
            Debug.Log("👁️ Mr. Mix has spotted the player! Begin eternal chase.");
            currentState = State.Chase;
            return;
        }

        if (!AIAgent.pathPending && AIAgent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            AIAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void ChaseBehavior(float distance)
    {
        AIAgent.isStopped = false;
        AIAgent.speed = chaseSpeed;

        if (!AIAgent.isOnNavMesh)
        {
            Debug.LogWarning("❌ Mr. Mix is NOT on the NavMesh!");
            return;
        }

        if (distance < attackRange)
        {
            currentState = State.Attack;
            return;
        }

        AIAgent.SetDestination(player.position);
        Debug.Log("🏃 Mr. Mix is chasing. Distance: " + distance);
    }

    void AttackBehavior(float distance)
    {
        AIAgent.ResetPath();
        AIAgent.speed = 0f;
        transform.LookAt(player);

        Debug.Log("💥 Mr. Mix is attacking!");
        if (distance > attackRange)
        {
            currentState = State.Chase;
        }
        
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;

        Debug.DrawRay(rayOrigin, directionToPlayer * chaseRange, Color.red);

        Ray ray = new Ray(rayOrigin, directionToPlayer);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, chaseRange))
        {
            Debug.Log("🔎 Ray hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("🎯 Player detected by ray!");
                return true;
            }
        }
        else
        {
            Debug.Log("❌ Ray hit nothing.");
        }

        return false;
    }
}
