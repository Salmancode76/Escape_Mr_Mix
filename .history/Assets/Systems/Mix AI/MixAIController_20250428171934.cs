using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;  // Required for ending the game session

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

    [Header("Movement Settings")]
    public float speedWalk = 2f;
    public float speedRun = 5f;

    [Header("Footstep SFX")]
    public AudioClip[] footstepClips;    
    public AudioClip bassClip;           
    public float walkFootstepRate = 0.5f;
    public float runFootstepRate  = 0.3f;
    private float footstepTimer;

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
        mrMixAnimator.SetBool("Attack", false);
    }

    void ChasePlayer()
    {
        if (detectedPlayerTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, detectedPlayerTransform.position);

        navMeshAgent.isStopped = false;

        if (distance <= 1f)
        {
            // End game when Mix is very close to the player (game ends immediately)
            EndGame();
        }
        else
        {
            navMeshAgent.speed = speedRun;
            navMeshAgent.SetDestination(detectedPlayerTransform.position);

            mrMixAnimator.SetBool("Run", true);
            mrMixAnimator.SetBool("Walk", false);
            mrMixAnimator.SetBool("Attack", false);

            RotateTowards(detectedPlayerTransform.position);
        }
    }

    void HandleFootsteps()
    {
        float speed = navMeshAgent.velocity.magnitude;

        if (speed > 0.1f)
        {
            footstepTimer += Time.deltaTime;

            // Adjust footstep rate based on speed (run should be faster)
            float interval = (speed >= speedRun - 0.1f) ? runFootstepRate : walkFootstepRate;

            if (footstepTimer >= interval)
            {
                footstepTimer = 0f;

                // Ensure detectedPlayerTransform is not null before calling PlayFootstepsBasedOnDistance
                if (detectedPlayerTransform != null)
                {
                    SoundFXManager.instance.PlayFootstepsBasedOnDistance(transform, detectedPlayerTransform, footstepClips, bassClip);
                }
            }
        }
        else
        {
            footstepTimer = 0f;
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

    // Method to end the game session when Mix is very close to the player
    void EndGame()
    {
        Debug.Log("Game Over! Mix has caught the player.");

        // Option 2: Quit the application (if no scene is needed)
        Application.Quit();
    }
}
