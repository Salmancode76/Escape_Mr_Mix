using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mr_Mix_Move : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRange = 10f;
    public float attackRange = 2f;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;

    private NavMeshAgent AIAgent;
    private int currentPatrolIndex = 0;
    private float stuckCheckTimer = 0f;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private bool hasSeenPlayer = false;

    // 🔊 Audio support
    public AudioSource themeMusicSource;
    public AudioSource chaseMusic;

    public AudioClip salman_level_chase;

    void Start()
    {
        AIAgent = GetComponent<NavMeshAgent>();

        // NavMesh settings
        AIAgent.speed = patrolSpeed;
        AIAgent.acceleration = 6f;
        AIAgent.angularSpeed = 360f;
        AIAgent.radius = 0.3f;
        AIAgent.stoppingDistance = 0.5f;
        AIAgent.autoBraking = false;
        AIAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        AIAgent.updateRotation = true;
        AIAgent.updateUpAxis = true;

        if (chaseMusic == null)
            chaseMusic = GetComponent<AudioSource>();

        if (patrolPoints.Length > 0)
            AIAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            if (!hasSeenPlayer)
            {
                Debug.Log("👁️ Mr. Mix spotted the player for the first time!");

                hasSeenPlayer = true;
                currentState = State.Chase;

                // Stop theme music if playing
                if (themeMusicSource != null && themeMusicSource.isPlaying)
                {
                    themeMusicSource.Stop();
                }

                // Play chase music forever
                if (chaseMusic != null && salman_level_chase != null)
                {
                    chaseMusic.clip = salman_level_chase;
                    chaseMusic.loop = true;
                    chaseMusic.Play();
                    Debug.Log("🎵 Chase music started forever!");
                }
            }
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

        if (!AIAgent.pathPending && AIAgent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            AIAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void ChaseBehavior(float distance)
    {
        AIAgent.speed = chaseSpeed;
        AIAgent.isStopped = false;

        if (!AIAgent.pathPending && Vector3.Distance(AIAgent.destination, player.position) > 1f)
        {
            AIAgent.SetDestination(player.position);
        }

        // Recalculate path if stuck
        stuckCheckTimer += Time.deltaTime;
        if (stuckCheckTimer >= 1.5f && AIAgent.velocity.sqrMagnitude < 0.1f)
        {
            Debug.Log("🔄 Recalculating path — Mr. Mix might be stuck.");
            AIAgent.ResetPath();
            AIAgent.SetDestination(player.position);
            stuckCheckTimer = 0f;
        }

        if (distance < attackRange)
        {
            currentState = State.Attack;
        }
    }

    void AttackBehavior(float distance)
    {
        AIAgent.isStopped = true;
        AIAgent.ResetPath();
        transform.LookAt(player);

        Debug.Log("💥 Mr. Mix is attacking!");

        if (distance > attackRange)
        {
            AIAgent.isStopped = false;
            currentState = State.Chase;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;

        Debug.DrawRay(rayOrigin, directionToPlayer * chaseRange, Color.red);

        if (Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, chaseRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("🎯 Player detected by ray!");
                return true;
            }
        }

        return false;
    }
}
