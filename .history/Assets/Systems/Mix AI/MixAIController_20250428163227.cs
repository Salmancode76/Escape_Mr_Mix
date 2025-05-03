using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MixAIController : MonoBehaviour
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

    [Header("Footstep SFX")]
    public AudioClip[] footstepClips;    // assign in inspector
    public AudioClip bassClip;           // assign in inspector
    public float walkFootstepRate = 0.5f;
    public float runFootstepRate  = 0.3f;
    private float footstepTimer;

    [Header("References")]
    public Animator mrMixAnimator;       // assign in inspector
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
            ChasePlayer();
        else
            Patroling();

        HandleFootsteps();
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            detectedPlayerTransform = hits[0].transform;
            isChasingPlayer = true;
            chaseTimer = chaseDuration;
        }
    }

    void Patroling()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;

        if (waypoints != null && waypoints.Length > 0)
        {
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_WaitTime <= 0f)
                {
                    m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                    m_WaitTime = startWaitTime;
                }
                else
                {
                    navMeshAgent.isStopped = true;
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }

        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
        mrMixAnimator.SetBool("Walk", isMoving);
        mrMixAnimator.SetBool("Run", false);
    }

    void ChasePlayer()
    {
        chaseTimer -= Time.deltaTime;
        float dist = Vector3.Distance(transform.position, detectedPlayerTransform.position);

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedRun;
        navMeshAgent.SetDestination(detectedPlayerTransform.position);

        mrMixAnimator.SetBool("Run", true);
        mrMixAnimator.SetBool("Walk", false);

        // attack toggle at 1 unit
        mrMixAnimator.SetBool("Attack", dist <= 1f);

        RotateTowards(detectedPlayerTransform.position);

        if (chaseTimer <= 0f)
        {
            isChasingPlayer = false;
            mrMixAnimator.SetBool("Run", false);
            mrMixAnimator.SetBool("Attack", false);
        }
    }

    void HandleFootsteps()
    {
        float speed = navMeshAgent.velocity.magnitude;
        if (speed > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            float interval = (speed > speedWalk) ? runFootstepRate : walkFootstepRate;

            if (footstepTimer >= interval)
            {
                footstepTimer = 0f;
                AudioClip step = footstepClips[Random.Range(0, footstepClips.Length)];
                SoundFXManager.instance.playSoundFXClip(step, transform, 1f);

                if (bassClip != null)
                    SoundFXManager.instance.playSoundFXClip(bassClip, transform, 1f);
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }
    }
}
