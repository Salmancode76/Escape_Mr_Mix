using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MixAIController : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;               //  Nav mesh agent component
    public float startWaitTime = 4;                 //  Wait time of every action
    public float timeToRotate = 2;                  //  Wait time when the enemy detect near the player without seeing
    public float speedWalk = 6;                     //  Walking speed, speed in the nav mesh agent
    public float speedRun = 9;                      //  Running speed

    public float viewRadius = 15;                   //  Radius of the enemy view
    public float viewAngle = 90;                    //  Angle of the enemy view
    public LayerMask playerMask;                    //  To detect the player with the raycast
    public LayerMask obstacleMask;                  //  To detect the obstacules with the raycast
    public float meshResolution = 1.0f;             //  How many rays will cast per degree
    public int edgeIterations = 4;                  //  Number of iterations to get a better performance of the mesh filter when the raycast hit an obstacule
    public float edgeDistance = 0.5f;               //  Max distance to calcule the a minumun and a maximum raycast when hits something

    public Transform[] waypoints;                   //  All the waypoints where the enemy patrols
    int m_CurrentWaypointIndex;                     //  Current waypoint where the enemy is going to

    Vector3 playerLastPosition = Vector3.zero;      //  Last position of the player when was near the enemy
    Vector3 m_PlayerPosition;                       //  Last position of the player when the player is seen by the enemy

    float m_WaitTime;                               //  Variable of the wait time that makes the delay
    float m_TimeToRotate;                           //  Variable of the wait time to rotate when the player is near that makes the delay
    bool m_playerInRange;                           //  If the player is in range of vision, state of chasing
    bool m_PlayerNear;                              //  If the player is near, state of hearing
    bool m_IsPatrol;                                //  If the enemy is patrol, state of patroling
    bool m_CaughtPlayer;                            //  if the enemy has caught the player

    // Reference to MrMix character, his Animator, and the Player
    public GameObject mrMix; // Assign in Inspector
    public Animator mrMixAnimator; // Assign in Inspector
    public Transform player; // Assign in Inspector
    public bool playerInRoom = false;

    // --- PLAYER DETECTION & CHASE TIMER FIELDS ---
    public LayerMask playerLayer; // Assign 'Player' layer in Inspector
    public float detectionRadius = 10f; // Detection radius for player
    private float chaseTimer = 0f;
    private bool isChasingPlayer = false;
    private Transform detectedPlayerTransform = null;

    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_playerInRange = false;
        m_PlayerNear = false;
        m_WaitTime = startWaitTime;                 //  Set the wait time variable that will change
        m_TimeToRotate = timeToRotate;

        m_CurrentWaypointIndex = 0;                 //  Set the initial waypoint
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;             //  Set the navemesh speed with the normal speed of the enemy
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);    //  Set the destination to the first waypoint
    }

    private void Update()
    {
        // Detect player by layer using OverlapSphere
        if (!isChasingPlayer)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
            if (hits.Length > 0)
            {
                // Player detected, start chasing for 15 seconds
                isChasingPlayer = true;
                chaseTimer = 15f;
                detectedPlayerTransform = hits[0].transform; // Store reference to the player
            }
        }

        if (isChasingPlayer)
        {
            chaseTimer -= Time.deltaTime;
            if (detectedPlayerTransform != null)
            {
                navMeshAgent.SetDestination(detectedPlayerTransform.position);
            }
            // Optionally, play chase/run animations here

            if (chaseTimer <= 0f)
            {
                isChasingPlayer = false;
                chaseTimer = 0f;
                detectedPlayerTransform = null;
                // Optionally, reset to patrol or other behavior
            }
        }
        else
        {
            // Normal patrol/chase logic (existing AI behavior)
            EnviromentView(); //  Check whether or not the player is in the enemy's field of vision
            if (!m_IsPatrol)
            {
                Chasing();
            }
            else
            {
                Patroling();
            }

            // Make MrMix follow the player if the player is in the room
            if (playerInRoom && mrMix != null && player != null)
            {
                float distance = Vector3.Distance(mrMix.transform.position, player.position);
                float speed = 1.0f; // Half the previous speed for MrMix
                float runThreshold = 5f;
                float hitThreshold = 1.5f;

                // Movement logic
                if (distance > hitThreshold)
                {
                    mrMix.transform.position = Vector3.MoveTowards(
                        mrMix.transform.position,
                        player.position,
                        speed * Time.deltaTime
                    );
                    Vector3 direction = (player.position - mrMix.transform.position).normalized;
                    if (direction != Vector3.zero)
                        mrMix.transform.forward = new Vector3(direction.x, 0, direction.z);
                }

                // Animation logic
                if (mrMixAnimator != null)
                {
                    if (distance > runThreshold)
                    {
                        mrMixAnimator.SetBool("isRunning", true);
                        mrMixAnimator.SetBool("isWalking", false);
                    }
                    else if (distance > hitThreshold)
                    {
                        mrMixAnimator.SetBool("isRunning", false);
                        mrMixAnimator.SetBool("isWalking", true);
                    }
                    else
                    {
                        mrMixAnimator.SetTrigger("hit");
                        mrMixAnimator.SetBool("isRunning", false);
                        mrMixAnimator.SetBool("isWalking", false);
                    }
                }
            }
        }
    }

    private void Chasing()
    {
        //  The enemy is chasing the player
        m_PlayerNear = false;                       //  Set false that hte player is near beacause the enemy already sees the player
        playerLastPosition = Vector3.zero;          //  Reset the player near position

        if (!m_CaughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_PlayerPosition);          //  set the destination of the enemy to the player location
        }
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)    //  Control if the enemy arrive to the player location
        {
                if (m_WaitTime <= 0 && !m_CaughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                //  Check if the enemy is not near to the player, returns to patrol after the wait time delay
                m_IsPatrol = true;
                m_PlayerNear = false;
                Move(speedWalk);
                m_TimeToRotate = timeToRotate;
                m_WaitTime = startWaitTime;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                    //  Wait if the current position is not the player position
                    Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    private void Patroling()
    {
        if (m_PlayerNear)
        {
            //  Check if the enemy detect near the player, so the enemy will move to that position
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                //  The enemy wait for a moment and then go to the last player position
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerNear = false;           //  The player is no near when the enemy is platroling
            playerLastPosition = Vector3.zero;

            // SAFETY CHECK
            if (waypoints != null && waypoints.Length > 0)
            {
                // Clamp index to valid range
                m_CurrentWaypointIndex = Mathf.Clamp(m_CurrentWaypointIndex, 0, waypoints.Length - 1);
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);    //  Set the enemy destination to the next waypoint

                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    //  If the enemy arrives to the waypoint position then wait for a moment and go to the next
                    if (m_WaitTime <= 0)
                    {
                        NextPoint();
                        Move(speedWalk);
                        m_WaitTime = startWaitTime;
                    }
                    else
                    {
                        Stop();
                        m_WaitTime -= Time.deltaTime;
                    }
                }
            }
            else
            {
                Debug.LogError("Waypoints array is empty or not assigned on " + gameObject.name);
            }
        }
    }

    private void OnAnimatorMove()
    {

    }

    public void NextPoint()
    {
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);   //  Make an overlap sphere around the enemy to detect the playermask in the view radius

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);          //  Distance of the enmy and the player
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    m_playerInRange = true;             //  The player has been seeing by the enemy and then the nemy starts to chasing the player
                    m_IsPatrol = false;                 //  Change the state to chasing the player
                }
                else
                {
                    /*
                     *  If the player is behind a obstacle the player position will not be registered
                     * */
                    m_playerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                /*
                 *  If the player is further than the view radius, then the enemy will no longer keep the player's current position.
                 *  Or the enemy is a safe zone, the enemy will no chase
                 * */
                m_playerInRange = false;                //  Change the sate of chasing
            }
            if (m_playerInRange)
            {
                /*
                 *  If the enemy no longer sees the player, then the enemy will go to the last position that has been registered
                 * */
                m_PlayerPosition = player.transform.position;       //  Save the player's current position if the player is in range of vision
            }
        }
    }

    // Detect when the player enters/exits the room
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRoom = true;
            if (mrMixAnimator != null)
                mrMixAnimator.SetBool("isWalking", true); // Start walking animation
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRoom = false;
            if (mrMixAnimator != null)
                mrMixAnimator.SetBool("isWalking", false); // Stop walking animation
        }
    }
}
