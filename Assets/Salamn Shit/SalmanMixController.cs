using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityTutorial.PlayerControl;
using UnityTutorial.Manager;

public class SalmanMixController : MonoBehaviour
{
    public enum BehaviorMode { OnFollow, OnDetect, Patrol, OnMahdiLevel }
    public enum AIState { Idle, Patrolling, Investigating, Chasing, CutScene }

    [Header("Player Reference")]
    [SerializeField] private GameObject playerObject;
    
    private Transform playerTransform;
    private PlayerController playerController;
    private InputManager inputManager;

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

    [Header("CutScene Settings")]
    public AudioClip cutSceneAudioClip;
    public float cutSceneAudioDelay = 3f;
    private float cutSceneAudioTimer;
    private bool isCutScenePlaying;

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
    
    [Header("Movement Settings")]
    public float idleSpeed = 0f;
    public float patrolSpeed = 2f;
    public float investigateSpeed = 3f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f;
    public float minDistanceToWaypoint = 0.5f;
    
    [Header("Heavy Footsteps")]
    public AudioSource levelMusic;
    public AudioClip chaseMusic;
    public AudioClip heavyFootstepsLoop;
    private AudioSource heavyFootstepsSource;
    private bool isPlayingHeavyFootsteps = false;
    
    [Header("Audio")]
    public AudioClip[] footstepClips;
    public AudioClip bassClip;
    public AudioClip detectionSound;
    public AudioClip suspiciousSound;
    public float walkFootstepRate = 0.5f;
    public float runFootstepRate = 0.3f;
    private float footstepTimer;
    private AudioSource audioSource;
    public AudioClip chaseTheme; 
    
    [Header("References")]
    public Animator animator;
    private NavMeshAgent navAgent;

    [Header("Debugging")]
    public bool showDebugVisuals = false;
    
    public bool IsPlayerRunning => inputManager != null && !inputManager.Crouch && inputManager.Run;
    public bool IsPlayerCrouching => inputManager != null && inputManager.Crouch;
    
    [Header("Death Handling")]
    public GameObject DeathUICanvas;
    public AudioClip DeadPlayerSFX, DeathUIMusicSFX;
    public AudioClip DeadPlayerSFX2, DeathUIMusicSFX2;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        // Create a separate audio source for heavy footsteps
        heavyFootstepsSource = gameObject.AddComponent<AudioSource>();
        heavyFootstepsSource.loop = true;
        heavyFootstepsSource.spatialBlend = 1f; // 3D sound
        heavyFootstepsSource.playOnAwake = false;
        heavyFootstepsSource.volume = 2.0f;
         heavyFootstepsSource.minDistance = 5f; // Make footsteps audible from further away
     heavyFootstepsSource.maxDistance = 30f; // Extend the maximum distance for hear
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.maxDistance = 15f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogWarning("No Animator found on Mix AI or its children!");
        }

        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogError("No player found with 'Player' tag!");
                enabled = false;
                return;
            }
        }

        playerTransform = playerObject.transform;
        
        if (behaviorMode == BehaviorMode.OnDetect || behaviorMode == BehaviorMode.OnMahdiLevel)
        {
            playerController = playerObject.GetComponent<PlayerController>();
            inputManager = playerObject.GetComponent<InputManager>();

            if (playerController == null)
                Debug.LogError("PlayerController component missing!");
            if (inputManager == null)
                Debug.LogError("InputManager component missing!");
        }

        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing!");
            enabled = false;
        }
    }


    private void Start()
    {
        waitTimeRemaining = waypointWaitTime;
        lastStateChangeTime = -stateChangeDelay;
        FindPlayer();
        
        if (behaviorMode == BehaviorMode.OnFollow && playerTransform != null)
        {
            lastKnownPlayerPosition = playerTransform.position;
            
            if (animator != null)
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                animator.SetBool("Cut", false);
                
                animator.CrossFade("Run", 0.1f, 0, 0f);
                animator.SetBool("Run", true);
                animator.speed = 1f;
                animator.Update(0f);
            }
            
            TransitionToState(AIState.Chasing);
            return;
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
            
            if (behaviorMode == BehaviorMode.OnDetect || behaviorMode == BehaviorMode.OnMahdiLevel)
            {
                playerController = player.GetComponent<PlayerController>();
                inputManager = player.GetComponent<InputManager>();
            }
        }
    }

   private void Update()
{
    // Don't do anything if the player is dead
    if (hasDied) return;

    if (behaviorMode == BehaviorMode.OnFollow && playerTransform != null)
    {
        lastKnownPlayerPosition = playerTransform.position;
        
        if (currentState != AIState.Chasing)
            TransitionToState(AIState.Chasing);
    }
    
    if (behaviorMode != BehaviorMode.OnFollow)
        DetectPlayer();
        
    UpdateCurrentState();
    HandleFootsteps();
    UpdateAnimations();

    if (behaviorMode == BehaviorMode.OnMahdiLevel && isCutScenePlaying && cutSceneAudioClip != null)
    {
        cutSceneAudioTimer -= Time.deltaTime;
        if (cutSceneAudioTimer <= 0f)
        {
            audioSource.PlayOneShot(cutSceneAudioClip, 0.7f);
            cutSceneAudioTimer = cutSceneAudioDelay;
        }
    }
}
    
    private void UpdateCurrentState()
    {
        switch (currentState)
        {
            case AIState.Idle: HandleIdleState(); break;
            case AIState.Patrolling: HandlePatrolState(); break;
            case AIState.Investigating: HandleInvestigateState(); break;
            case AIState.Chasing: HandleChaseState(); break;
            case AIState.CutScene: HandleCutSceneState(); break;
        }
    }
    
    private void HandleIdleState()
    {
        navAgent.isStopped = true;
        waitTimeRemaining -= Time.deltaTime;
        
        if (behaviorMode == BehaviorMode.OnMahdiLevel)
        {
            TransitionToState(AIState.CutScene);
            return;
        }
        
        if (waitTimeRemaining <= 0)
        {
            waitTimeRemaining = waypointWaitTime;
            TransitionToState(AIState.Patrolling);
        }
    }
    
    private void HandleCutSceneState()
    {
        if (behaviorMode != BehaviorMode.OnMahdiLevel)
        {
            TransitionToState(AIState.Patrolling);
            return;
        }

        navAgent.isStopped = true;
        waitTimeRemaining -= Time.deltaTime;
        
        if (animator != null)
            animator.SetBool("Cut", true);
        
        isCutScenePlaying = true;
        
        if (waitTimeRemaining <= 0)
        {
            if (animator != null)
                animator.SetBool("Cut", false);
            
            isCutScenePlaying = false;
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
            
            if (behaviorMode == BehaviorMode.OnMahdiLevel)
            {
                cutSceneAudioTimer = 0f;
                TransitionToState(AIState.CutScene);
            }
            else
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
    // Don't do anything if player is dead
    if (hasDied)
    {
        navAgent.isStopped = true;
        return;
    }

    if (playerTransform == null)
    {
        TransitionToState(AIState.Patrolling);
        return;
    }

    navAgent.isStopped = false;
    navAgent.speed = chaseSpeed;
    navAgent.acceleration = 20f;
    navAgent.autoBraking = false;

    // Always set destination to player position regardless of player movement
    navAgent.SetDestination(playerTransform.position);
    
    // Play heavy footsteps loop if not already playing
    if (!isPlayingHeavyFootsteps && heavyFootstepsLoop != null && heavyFootstepsSource != null)
    {
        heavyFootstepsSource.clip = heavyFootstepsLoop;
        heavyFootstepsSource.Play();
        isPlayingHeavyFootsteps = true;
    }
    
    // Check distance to player regardless of player movement state
    Vector3 flatMixPosition = new Vector3(transform.position.x, 0, transform.position.z);
    Vector3 flatPlayerPosition = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
    
    // Calculate direct distance on XZ plane (ignoring Y axis height difference)
    float directDistance = Vector3.Distance(flatMixPosition, flatPlayerPosition);
    
    // More aggressive kill detection
    if (directDistance <= 2.0f)  // Increased kill radius from 1.5f to 2.0f
    {
        // Instant kill, no delay
        InstantKillPlayer();
        return;
    }
    
    // Check even more aggressively for more precise collision detection
    Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
    RaycastHit hit;
    
    // Cast a wider ray to detect player
    if (Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.75f, directionToPlayer, out hit, 2.5f, playerLayer))
    {
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Player detected by spherecast, immediate kill
            InstantKillPlayer();
        }
    }
}
    
   private bool hasDied = false;
public void InstantKillPlayer()
{
    if (hasDied) return; // Prevent repeated calls
    hasDied = true;

    // Stop the heavy footsteps
    if (heavyFootstepsSource != null)
    {
        heavyFootstepsSource.loop = false;
        heavyFootstepsSource.volume = 0; 
        heavyFootstepsSource.mute = true;
        heavyFootstepsSource.Stop();
        isPlayingHeavyFootsteps = false;
    }

    // Stop chase music specifically
    if (levelMusic != null && chaseMusic != null && levelMusic.clip == chaseMusic)
    {
        levelMusic.Stop();
    }
    
    // Find and stop all audio sources playing the salman_level_theme
    AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
    foreach (AudioSource audio in allAudioSources)
    {
        if (audio.clip != null && audio.clip.name == "salman_level_theme")
        {
            audio.Stop();
        }
    }

    if (DeathUICanvas != null)
    {
        DeathUICanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (DeathUICanvas.TryGetComponent<CanvasGroup>(out var cg)) 
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    // Change the state to stop chasing
    currentState = AIState.Idle;
    navAgent.isStopped = true;

    Time.timeScale = 0;

    if (DeadPlayerSFX != null &&DeadPlayerSFX2 !=null )
    {
        SoundFXManager.instance.playSoundFXClip(DeadPlayerSFX2, transform, 0.65f);
        SoundFXManager.instance.playSoundFXClipLooped(DeathUIMusicSFX2, transform, 1f);

        SoundFXManager.instance.playSoundFXClip(DeadPlayerSFX, transform, 0.65f);
        SoundFXManager.instance.playSoundFXClipLooped(DeathUIMusicSFX, transform, 1f);
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
    
    if ((behaviorMode == BehaviorMode.OnDetect || behaviorMode == BehaviorMode.OnMahdiLevel) && inputManager != null)
    {
        if (IsPlayerCrouching) 
        {
            effectiveVisionRadius *= crouchVisionRadiusModifier;
            effectiveHearingRadius *= crouchHearingRadiusModifier;
            effectiveImmediateRadius *= crouchImmediateDetectionModifier;
        }
        else if (IsPlayerRunning)
            effectiveHearingRadius *= runningNoiseMultiplier;
    }
    
    // First check: immediate detection radius - works for all player states
    if (distanceToPlayer <= effectiveImmediateRadius)
    {
        hasLineOfSight = CheckLineOfSight();
        if (hasLineOfSight)
        {
            OnPlayerDetected();
            return;
        }
    }
    
    // Second check: vision detection - works for all player states
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
    
    // Third check: hearing detection - modified to also detect idle players at closer range
    if (distanceToPlayer <= effectiveHearingRadius)
    {
        bool isPlayerMakingNoise = false;
        
        if ((behaviorMode == BehaviorMode.OnDetect || behaviorMode == BehaviorMode.OnMahdiLevel) && inputManager != null)
        {
            // Modified: Now detects idle players when they're close enough
            isPlayerMakingNoise = IsPlayerRunning || 
                                 (distanceToPlayer <= effectiveHearingRadius * 0.5f);
        }
        else
        {
            // For other behavior modes, use simpler distance check
            isPlayerMakingNoise = (distanceToPlayer <= effectiveHearingRadius * 0.7f);
        }
        
        if (isPlayerMakingNoise)
            BecomeAwareOfPlayer();
    }
    
    hasLineOfSight = false;
}
    
private void BecomeAwareOfPlayer()
{
    lastKnownPlayerPosition = playerTransform.position;

    if (currentState != AIState.Chasing && suspiciousSound != null)
        audioSource.PlayOneShot(suspiciousSound, 0.5f);

    // Always trigger full sprint chase logic, even for idle players
    // This ensures that once detected, all players are chased properly
    if (behaviorMode == BehaviorMode.OnDetect || behaviorMode == BehaviorMode.OnMahdiLevel)
    {
        // OnPlayerDetected() triggers the full sprint chase logic
        OnPlayerDetected();
        return;
    }
    else if (Vector3.Distance(transform.position, playerTransform.position) <= immediateDetectionRadius * 1.2f)
    {
        // If player is close enough, directly transition to chase state
        // This works for all player states including idle
        OnPlayerDetected();
        return;
    }

    // For more distant detections, go to investigating state first
    TransitionToState(AIState.Investigating);
}


    
    private bool CheckLineOfSight()
    {
        if (playerTransform == null) return false;
            
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, directionToPlayer.normalized, 
            out hit, distanceToPlayer, obstacleLayer | playerLayer))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if ((behaviorMode == BehaviorMode.OnDetect || behaviorMode == BehaviorMode.OnMahdiLevel) && 
                    inputManager != null && IsPlayerCrouching)
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
        if (currentState != AIState.Chasing)
        {
            if (levelMusic != null && chaseMusic != null)
            {
                levelMusic.clip = chaseMusic;
                levelMusic.Play();
            }
            
            if (detectionSound != null)
                audioSource.PlayOneShot(detectionSound, 0.45f);
                
            lastKnownPlayerPosition = playerTransform.position;
            
            navAgent.acceleration = 20f;
            navAgent.autoBraking = false;
            navAgent.speed = chaseSpeed;

            TransitionToState(AIState.Chasing);
            
            if (animator != null)
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Cut", false);
                animator.CrossFade("Run", 0.1f, 0, 0f);
                animator.SetBool("Run", true);
                animator.speed = 1f;
                animator.Update(0f);
            }
        }
    }

    private void TransitionToState(AIState newState)
    {
        // Stop heavy footsteps when no longer chasing
        if (currentState == AIState.Chasing && newState != AIState.Chasing)
        {
            if (isPlayingHeavyFootsteps)
            {
                heavyFootstepsSource.Stop();
                isPlayingHeavyFootsteps = false;
            }
            
            if (levelMusic != null && levelMusic.clip == chaseMusic)
            {
                // Return to normal music
                AudioSource levelAudio = GameObject.Find("3- Salman")?.GetComponent<AudioSource>();
                if (levelAudio != null)
                {
                    levelMusic.Stop();
                    levelMusic.Play();
                }
            }
        }
 
        if (Time.time - lastStateChangeTime < stateChangeDelay)
            return;
            
        lastStateChangeTime = Time.time;
        
        switch (currentState)
        {
            case AIState.CutScene: 
                animator.SetBool("Cut", false);
                isCutScenePlaying = false;
                break;
        }
        
        switch (newState)
        {
            case AIState.Idle: navAgent.isStopped = true; break;
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
            case AIState.CutScene:
                navAgent.isStopped = true;
                cutSceneAudioTimer = 0f;
                isCutScenePlaying = true;
                break;
        }
        
        currentState = newState;

        if (animator != null && newState == AIState.Chasing)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Cut", false);
            animator.SetBool("Run", true);
            animator.CrossFade("Run", 0.1f, 0, 0f);
            animator.speed = 1f;
            animator.Update(0f);
        }
    }
    
    private void HandleFootsteps()
    {
        float speed = navAgent.velocity.magnitude;
        
        // Check if we should be playing heavy footsteps (when chasing)
        bool shouldPlayHeavyFootsteps = (currentState == AIState.Chasing && speed > 0.1f);
        
        // Handle normal footsteps (only when NOT chasing)
        if (speed > 0.1f && currentState != AIState.Chasing)
        {
            footstepTimer += Time.deltaTime;
            
            float interval = walkFootstepRate;
            
            if (footstepTimer >= interval)
            {
                footstepTimer = 0f;
                
                if (footstepClips != null && footstepClips.Length > 0)
                {
                    AudioClip footstepClip = footstepClips[Random.Range(0, footstepClips.Length)];
                    SoundFXManager.instance.PlayFootstepsBasedOnDistance(footstepClip, transform, 0.5f);
                }
                
                if (bassClip != null)
                    SoundFXManager.instance.PlayFootstepsBasedOnDistance(bassClip, transform, 0.5f);
            }
        }
        else if (speed <= 0.1f)
        {
            footstepTimer = 0f;
        }
    }
    
    private AIState lastAnimationState;
    private bool wasMoving;
    
    private void UpdateAnimations()
    {
        bool isMoving = navAgent.velocity.magnitude > 0.1f;
        bool shouldRun = currentState == AIState.Chasing || 
                 (behaviorMode == BehaviorMode.OnDetect || behaviorMode == BehaviorMode.OnMahdiLevel);


        if (currentState != lastAnimationState || isMoving != wasMoving)
        {
            animator.SetBool("Walk", isMoving && !shouldRun);
            animator.SetBool("Run", shouldRun && isMoving);
            
            if (currentState == AIState.CutScene)
                animator.SetBool("Cut", true);
            else
                animator.SetBool("Cut", false);
            
            if (shouldRun && isMoving)
            {
                animator.Play("Run", 0, 0f);
                animator.speed = 1f;
            }
        }
        
        lastAnimationState = currentState;
        wasMoving = isMoving;
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
        if (waypoints == null || waypoints.Length <= 1) return;
            
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnDisable()
    {
        // Make sure to stop any playing sounds when disabled
        if (isPlayingHeavyFootsteps && heavyFootstepsSource != null)
        {
            heavyFootstepsSource.Stop();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showDebugVisuals) return;

        if (behaviorMode == BehaviorMode.OnFollow && playerTransform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        } 

        float effectiveVisionRadius = visionRadius;
        float effectiveHearingRadius = hearingRadius;
        float effectiveImmediateRadius = immediateDetectionRadius;
        
        if ((behaviorMode == BehaviorMode.OnDetect || behaviorMode == BehaviorMode.OnMahdiLevel) && inputManager != null)
        {
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
                Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        }
        else
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        
        Gizmos.DrawWireSphere(transform.position, effectiveVisionRadius);
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, effectiveHearingRadius);
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, effectiveImmediateRadius);
        
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