using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityTutorial.PlayerControl;
using UnityTutorial.Manager;

public class MixAIController : MonoBehaviour
{
    public enum BehaviorMode { OnFollow, OnDetect, Patrol }
    public enum AIState { Idle, Patrolling, Investigating, Chasing, Attacking }

    [Header("Behavior Settings")]
    public BehaviorMode behaviorMode = BehaviorMode.OnDetect;
    [SerializeField] private AIState currentState = AIState.Patrolling;
    [SerializeField] private float stateChangeDelay = 0.5f;
    private float lastStateChangeTime;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex;
    public float waypointWaitTime = 2f;
    private float waitTimeRemaining;
    public bool randomizePatrolOrder = false;

    [Header("Detection Settings")]
    public float visionRadius = 12f;
    public float visionAngle = 90f;
    public float hearingRadius = 8f;
    public float immediateDetectionRadius = 2f;
    
    [Header("Stealth Detection Modifiers")]
    public float crouchVisionRadiusModifier = 0.5f;
    public float crouchHearingRadiusModifier = 0.3f;
    public float crouchImmediateDetectionModifier = 0.7f;
    public float runningNoiseMultiplier = 1.5f;
    
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public float losePlayerTime = 5f;
    private float losePlayerTimer;
    [SerializeField] private Vector3 lastKnownPlayerPosition;
    [SerializeField] private bool hasLineOfSight;
    private Transform playerTransform;
    private PlayerController playerController;
    private InputManager inputManager;
    
    [Header("Movement Settings")]
    public float idleSpeed = 0f;
    public float patrolSpeed = 2f;
    public float investigateSpeed = 3f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f;
    public float minDistanceToWaypoint = 0.5f;
    
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;
    
    [Header("Audio")]
    public AudioClip[] footstepClips;
    public AudioClip bassClip;
    public AudioClip detectionSound;
    public AudioClip suspiciousSound;
    public AudioClip attackSound;
    public float walkFootstepRate = 0.5f;
    public float runFootstepRate = 0.3f;
    private float footstepTimer;
    private AudioSource audioSource;

    [Header("References")]
    public Animator animator;
    private NavMeshAgent navAgent;

    [Header("Debugging")]
    public bool showDebugVisuals = false;
    
    // Properties to check player state
    public bool IsPlayerRunning => inputManager != null && !inputManager.Crouch && inputManager.Run;
    public bool IsPlayerCrouching => inputManager != null && inputManager.Crouch;
    
    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.maxDistance = 15f;
        }
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        waitTimeRemaining = waypointWaitTime;
        lastStateChangeTime = -stateChangeDelay;
        FindPlayer();
        
        if (behaviorMode == BehaviorMode.OnFollow && playerTransform != null)
        {
            TransitionToState(AIState.Chasing);
        }
        
        if (randomizePatrolOrder && waypoints != null && waypoints.Length > 1)
            ShuffleWaypoints();
            
        TransitionToState(AIState.Patrolling);
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerController = player.GetComponent<PlayerController>();
            inputManager = player.GetComponent<InputManager>();
        }
    }

    private void Update()
    {
        if (behaviorMode != BehaviorMode.OnFollow)
            DetectPlayer();
            
        UpdateCurrentState();
        HandleFootsteps();
        UpdateAnimations();
    }
    
    private void UpdateCurrentState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                HandleIdleState();
                break;
            case AIState.Patrolling:
                HandlePatrolState();
                break;
            case AIState.Investigating:
                HandleInvestigateState();
                break;
            case AIState.Chasing:
                HandleChaseState();
                break;
            case AIState.Attacking:
                HandleAttackState();
                break;
        }
    }
    
    private void HandleIdleState()
    {
        navAgent.isStopped = true;
        waitTimeRemaining -= Time.deltaTime;
        
        if (waitTimeRemaining <= 0)
        {
            waitTimeRemaining = waypointWaitTime;
            TransitionToState(AIState.Patrolling);
        }
    }
    
    private void HandlePatrolState()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            TransitionToState(AIState.Idle);
            return;
        }
        
        navAgent.isStopped = false;
        navAgent.speed = patrolSpeed;
        
        if (navAgent.destination != waypoints[currentWaypointIndex].position)
            navAgent.SetDestination(waypoints[currentWaypointIndex].position);
            
        if (!navAgent.pathPending && navAgent.remainingDistance <= minDistanceToWaypoint)
        {
            waitTimeRemaining = waypointWaitTime;
            TransitionToState(AIState.Idle);
            
            if (randomizePatrolOrder)
                ShuffleWaypoints();
            else
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
    
    private void HandleInvestigateState()
    {
        navAgent.isStopped = false;
        navAgent.speed = investigateSpeed;
        
        if (navAgent.destination != lastKnownPlayerPosition)
            navAgent.SetDestination(lastKnownPlayerPosition);
            
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            StartCoroutine(LookAround());
            TransitionToState(AIState.Patrolling);
        }
    }
    
    private void HandleChaseState()
    {
        if (playerTransform == null)
        {
            TransitionToState(AIState.Patrolling);
            return;
        }
        
        navAgent.isStopped = false;
        navAgent.speed = chaseSpeed;
        
        if (hasLineOfSight)
        {
            lastKnownPlayerPosition = playerTransform.position;
            losePlayerTimer = 0;
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            
            if (losePlayerTimer >= losePlayerTime)
            {
                TransitionToState(AIState.Investigating);
                return;
            }
        }
        
        navAgent.SetDestination(lastKnownPlayerPosition);
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange && hasLineOfSight)
        {
            TransitionToState(AIState.Attacking);
        }
    }
    
    private void HandleAttackState()
    {
        if (playerTransform == null)
        {
            TransitionToState(AIState.Patrolling);
            return;
        }
        
        RotateTowards(playerTransform.position);
        navAgent.isStopped = true;
        
        float timeSinceLastAttack = Time.time - lastAttackTime;
        if (timeSinceLastAttack >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > attackRange)
        {
            TransitionToState(AIState.Chasing);
        }
    }
    
    private void DetectPlayer()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        float effectiveVisionRadius = visionRadius;
        float effectiveHearingRadius = hearingRadius;
        float effectiveImmediateRadius = immediateDetectionRadius;
        
        if (IsPlayerCrouching) 
        {
            effectiveVisionRadius *= crouchVisionRadiusModifier;
            effectiveHearingRadius *= crouchHearingRadiusModifier;
            effectiveImmediateRadius *= crouchImmediateDetectionModifier;
        }
        else if (IsPlayerRunning)
        {
            effectiveHearingRadius *= runningNoiseMultiplier;
        }
        
        if (distanceToPlayer <= effectiveImmediateRadius)
        {
            hasLineOfSight = CheckLineOfSight();
            if (hasLineOfSight)
            {
                OnPlayerDetected();
                return;
            }
        }
        
        if (distanceToPlayer <= effectiveVisionRadius)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            
            if (angleToPlayer <= visionAngle / 2)
            {
                hasLineOfSight = CheckLineOfSight();
                if (hasLineOfSight)
                {
                    OnPlayerDetected();
                    return;
                }
            }
        }
        
        if (distanceToPlayer <= effectiveHearingRadius)
        {
            bool isPlayerMakingNoise = IsPlayerRunning;
            
            if (isPlayerMakingNoise)
            {
                BecomeAwareOfPlayer();
            }
        }
        
        hasLineOfSight = false;
    }
    
    private void BecomeAwareOfPlayer()
    {
        lastKnownPlayerPosition = playerTransform.position;
        
        if (currentState != AIState.Chasing && currentState != AIState.Attacking && suspiciousSound != null)
        {
            audioSource.PlayOneShot(suspiciousSound, 0.5f);
        }
        
        TransitionToState(AIState.Investigating);
    }
    
    private bool CheckLineOfSight()
    {
        if (playerTransform == null)
            return false;
            
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, directionToPlayer.normalized, out hit, distanceToPlayer, obstacleLayer | playerLayer))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (IsPlayerCrouching)
                {
                    float stealthFactor = distanceToPlayer / visionRadius;
                    float detectionChance = (1 - stealthFactor) * (1 - crouchVisionRadiusModifier);
                    return Random.value <= detectionChance;
                }
                return true;
            }
        }
        return false;
    }
    
    private void OnPlayerDetected()
    {
        if (currentState != AIState.Chasing && currentState != AIState.Attacking)
        {
            if (detectionSound != null)
                audioSource.PlayOneShot(detectionSound, 0.7f);
                
            lastKnownPlayerPosition = playerTransform.position;
            TransitionToState(AIState.Chasing);
        }
    }
    
    private void Attack()
    {
        animator.SetTrigger("Attack");
        
        if (attackSound != null)
            audioSource.PlayOneShot(attackSound, 0.7f);
        
        if (playerTransform != null && 
            Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            EndGame();
        }
    }
    
    private void TransitionToState(AIState newState)
    {
        if (Time.time - lastStateChangeTime < stateChangeDelay)
            return;
            
        lastStateChangeTime = Time.time;
        
        switch (currentState)
        {
            case AIState.Attacking:
                animator.ResetTrigger("Attack");
                break;
        }
        
        switch (newState)
        {
            case AIState.Idle:
                navAgent.isStopped = true;
                break;
            case AIState.Patrolling:
                navAgent.speed = patrolSpeed;
                navAgent.isStopped = false;
                break;
            case AIState.Investigating:
                navAgent.speed = investigateSpeed;
                navAgent.isStopped = false;
                break;
            case AIState.Chasing:
                navAgent.speed = chaseSpeed;
                navAgent.isStopped = false;
                break;
            case AIState.Attacking:
                navAgent.isStopped = true;
                break;
        }
        
        currentState = newState;
    }
    
    private void HandleFootsteps()
    {
        float speed = navAgent.velocity.magnitude;
        
        if (speed > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            
            float interval = (currentState == AIState.Chasing) ? runFootstepRate : walkFootstepRate;
            
            if (footstepTimer >= interval)
            {
                footstepTimer = 0f;
                
                if (footstepClips != null && footstepClips.Length > 0)
                {
                    AudioClip footstepClip = footstepClips[Random.Range(0, footstepClips.Length)];
                    SoundFXManager.instance.PlayFootstepsBasedOnDistance(footstepClip, transform, 0.5f);
                }
                
                if (bassClip != null)
                {
                    SoundFXManager.instance.PlayFootstepsBasedOnDistance(bassClip, transform, 0.5f);
                }
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
    
    private void UpdateAnimations()
    {
        bool isMoving = navAgent.velocity.magnitude > 0.1f;
        bool isRunning = currentState == AIState.Chasing;
        bool isAttacking = currentState == AIState.Attacking;
        
        animator.SetBool("Walk", isMoving && !isRunning);
        animator.SetBool("Run", isRunning);
        animator.SetBool("Attack", isAttacking);
    }
    
    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0f;
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
    
    private void ShuffleWaypoints()
    {
        if (waypoints == null || waypoints.Length <= 1)
            return;
            
        int newIndex = currentWaypointIndex;
        while (newIndex == currentWaypointIndex)
            newIndex = Random.Range(0, waypoints.Length);
            
        currentWaypointIndex = newIndex;
    }
    
    private IEnumerator LookAround()
    {
        float lookAroundDuration = 2f;
        float startTime = Time.time;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, Random.Range(120f, 240f), 0);
        
        while (Time.time < startTime + lookAroundDuration)
        {
            float t = (Time.time - startTime) / lookAroundDuration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }
    }
    
    private void EndGame()
    {
        Debug.Log("Game Over! Player caught by Mix.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showDebugVisuals)
            return;
            
        float effectiveVisionRadius = visionRadius;
        float effectiveHearingRadius = hearingRadius;
        float effectiveImmediateRadius = immediateDetectionRadius;
        
        if (IsPlayerCrouching) 
        {
            effectiveVisionRadius *= crouchVisionRadiusModifier;
            effectiveHearingRadius *= crouchHearingRadiusModifier;
            effectiveImmediateRadius *= crouchImmediateDetectionModifier;
            Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.2f);
        }
        else if (IsPlayerRunning)
        {
            effectiveHearingRadius *= runningNoiseMultiplier;
            Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.2f);
        }
        else
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        }
        
        Gizmos.DrawWireSphere(transform.position, effectiveVisionRadius);
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, effectiveHearingRadius);
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, effectiveImmediateRadius);
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Vector3 forward = transform.forward * effectiveVisionRadius;
        float halfAngle = visionAngle / 2;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;
        
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);
        
        int segments = 20;
        Vector3 prevPoint = transform.position + leftRayDirection;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + i * visionAngle / segments;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 nextPoint = transform.position + rotation * forward;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
        
        if (waypoints != null && waypoints.Length > 0 && currentWaypointIndex < waypoints.Length)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(waypoints[currentWaypointIndex].position, 0.3f);
            Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex].position);
        }
        
        if (currentState == AIState.Investigating || currentState == AIState.Chasing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastKnownPlayerPosition, 0.3f);
            Gizmos.DrawLine(transform.position, lastKnownPlayerPosition);
        }
    }
}