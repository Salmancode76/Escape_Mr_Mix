using UnityEngine;
using UnityEngine.AI;

public class MrMixAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int m_CurrentWaypointIndex;
    public float startWaitTime = 2f;
    private float m_WaitTime;

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public LayerMask playerLayer;
    private Transform detectedPlayerTransform;
    private bool isChasingPlayer;
    public float chaseDuration = 5f;
    private float chaseTimer;

    [Header("Movement Settings")]
    public float speedWalk = 2f;
    public float speedRun = 5f;
    public float timeToRotate = 2f;
    private float m_TimeToRotate;
    private bool m_PlayerNear;

    [Header("References")]
    public Animator mrMixAnimator;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        m_WaitTime = startWaitTime;
    }

    void Update()
    {
        DetectPlayer();

        if (isChasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            Patroling();
        }
    }

    void DetectPlayer()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (players.Length > 0)
        {
            detectedPlayerTransform = players[0].transform;
            isChasingPlayer = true;
            chaseTimer = chaseDuration;
        }
    }

    void Patroling()
    {
        if (m_PlayerNear)
        {
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(detectedPlayerTransform.position);
            }
            else
            {
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            if (waypoints != null && waypoints.Length > 0)
            {
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (m_WaitTime <= 0)
                    {
                        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                        m_WaitTime = startWaitTime;
                    }
                    else
                    {
                        Stop();
                        m_WaitTime -= Time.deltaTime;
                    }
                }
                else
                {
                    Move(speedWalk);
                }
            }
        }

        if (mrMixAnimator != null)
        {
            bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
            mrMixAnimator.SetBool("Walk", isMoving);
            mrMixAnimator.SetBool("Run", false);
        }
    }

    void ChasePlayer()
    {
        chaseTimer -= Time.deltaTime;

        if (detectedPlayerTransform != null)
        {
            navMeshAgent.SetDestination(detectedPlayerTransform.position);

            float distanceToPlayer = Vector3.Distance(transform.position, detectedPlayerTransform.position);

            if (mrMixAnimator != null)
            {
                mrMixAnimator.SetBool("Run", true);
                mrMixAnimator.SetBool("Walk", false);

                if (distanceToPlayer <= 1.5f)
                {
                    mrMixAnimator.SetTrigger("Attack");
                }
            }

            // Optional: Rotate toward player
            RotateTowards(detectedPlayerTransform.position);
        }

        if (chaseTimer <= 0f)
        {
            isChasingPlayer = false;
            chaseTimer = 0f;
            detectedPlayerTransform = null;

            if (mrMixAnimator != null)
            {
                mrMixAnimator.SetBool("Run", false);
                mrMixAnimator.SetBool("Walk", false);
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;

        if (mrMixAnimator != null && !isChasingPlayer)
            mrMixAnimator.SetBool("Walk", true);
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0f;

        if (mrMixAnimator != null)
        {
            mrMixAnimator.SetBool("Walk", false);
            mrMixAnimator.SetBool("Run", false);
        }
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void LookingPlayer(Vector3 player)
    {
        Vector3 direction = (player - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
