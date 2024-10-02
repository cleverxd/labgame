using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy tweaking")]
    public float patrolSpeed = 2f;       // Speed while patrolling
    public float chaseSpeed = 4f;        // Speed while chasing
    public float visionRange = 10f;      // Vision range for detecting player
    public float visionAngle = 90f;      // Vision angle for detecting player
    public float patrolRadius = 20f;     // Radius for choosing random patrol points
    public float waitTimeAtPoint = 2f;   // Time to wait at a patrol point before moving to the next
    public LayerMask playerLayer;        // Player layer for detection
    public Transform player;             // Reference to the player
    public NavMeshAgent agent;           // Reference to the NavMeshAgent component
    
    [Header("Other scripts")]
    public Timer timer;
    private EndScript endScript;
    private PlayerAudio playerAudio;

    private bool isChasing = false;      // State of whether the enemy is chasing the player
    private bool playerChased = false;

    private void Awake()
    {
        playerAudio = FindFirstObjectByType<PlayerAudio>();
        endScript = FindFirstObjectByType<EndScript>();
    }

    private void Start()
    {
        // Initialize the NavMeshAgent speed and start patrolling
        agent.speed = patrolSpeed;
        StartCoroutine(Patrol());
    }

    private void Update()
    {
        // Check for the player in the enemy's vision
        if (CanSeePlayer())
        {
            // Switch to chasing state
            if (!isChasing)
            {
                isChasing = true;
                agent.speed = chaseSpeed;  // Increase speed for chasing
            }

            // Chase the player
            agent.SetDestination(player.position);
        }
        else
        {
            // If we lose sight of the player, go back to patrolling
            if (isChasing)
            {
                isChasing = false;
                agent.speed = patrolSpeed; // Go back to patrol speed
                StartCoroutine(Patrol());  // Resume patrol after losing player
            }
        }
    }

    private IEnumerator Patrol()
    {
        while (!isChasing)  // Only patrol if not chasing
        {
            Vector3 randomPatrolPoint = GetRandomPointOnNavMesh();
            agent.SetDestination(randomPatrolPoint);

            // Wait until the agent reaches the patrol point
            while (agent.pathPending || (agent.isActiveAndEnabled && agent.isOnNavMesh && agent.remainingDistance > 0.5f))
            {
                yield return null;
            }

            // Wait for a few seconds at the patrol point
            yield return new WaitForSeconds(waitTimeAtPoint);
        }
    }


    private Vector3 GetRandomPointOnNavMesh()
    {
        // Choose a random direction and point within a given radius
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;  // Move the point to be around the enemy's current position

        // Find a valid point on the NavMesh in that direction
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // If no valid point is found, return the current position
        return transform.position;
    }

    private bool CanSeePlayer()
    {
        // Get the direction to the player
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Check if the player is within vision range
        if (distanceToPlayer < visionRange)
        {
            // Check if the player is within the enemy's field of vision
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer < visionAngle / 2f)
            {
                // Perform a raycast to check if the player is obstructed
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, visionRange, playerLayer))
                {
                    if (hit.transform == player)
                    {
                        return true; // Player is in sight
                    }
                }
            }
        }

        return false; // Player is not in sight
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!playerChased)
            {
                timer.StopTimer();
                playerAudio.audioSource.PlayOneShot(playerAudio.playerDying);
                StartCoroutine(endScript.LoseByEnemy());
            }

            playerChased = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the vision cone in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 forward = transform.forward * visionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
