using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public Transform player; // Player's position
    public NavMeshAgent agent; // Zombie's NavMesh agent for pathfinding
    private Animator animator; // To handle animations
    public float attackDistance = 1.5f; // Distance to trigger attack
    public float attackInterval = 1f; // Time between attacks
    public float attackAnimationDuration = 1f; // Duration of the attack animation
    public float health = 100f; // Health value for the zombie
    private bool isAttacking = false; // Flag to control attack coroutine
    private float nextAttackTime = 0f; // Tracks time for the next attack
    private ZombieSpawner spawner; // Reference to the ZombieSpawner
    public bool isFalling = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get NavMeshAgent
        animator = GetComponent<Animator>(); // Get Animator
        spawner = FindObjectOfType<ZombieSpawner>(); // Get the ZombieSpawner in the scene
    }

    void Update()
    {
        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    
        // If the zombie is falling, do not process further
        if (isFalling) return;
        
        // Update Animator distance
        animator.SetFloat("Distance", distanceToPlayer);
        
        // Handle movement towards the player
        if (distanceToPlayer > attackDistance)
        {
            if (!isAttacking)
            {
                agent.isStopped = false; // Ensure the agent is moving
                animator.SetBool("IsRunning", true); // Trigger run animation
                agent.SetDestination(player.position); // Keep moving towards the player
            }
        }
        else
        {
            // Zombie is close enough to attack
            animator.SetBool("IsRunning", false); // Stop running animation
            if (!isAttacking && Time.time >= nextAttackTime)
            {
                // Start the attack coroutine if it's not already running
                StartCoroutine(AttackPlayer());
            }
        }

        // Check if health is 0 or less and handle death
        if (health <= 0)
        {
            Die();
        }
    }

    public int GetDamage()
    {
        return Random.Range(1, 14); // Random.Range with an inclusive lower bound and exclusive upper bound
    }

    // Coroutine for handling attacking behavior
    IEnumerator AttackPlayer()
    {
        isAttacking = true; // Set attacking flag
        agent.isStopped = true; // Stop the agent from moving while attacking
        animator.SetTrigger("Attack"); // Trigger attack animation

        // Wait for the attack animation to finish playing (set the duration of the animation)
        yield return new WaitForSeconds(attackAnimationDuration);

        // Set the next attack time to be current time + interval, allowing dynamic adjustment
        nextAttackTime = Time.time + attackInterval;

        // After the attack, reset the attacking flag and resume movement
        isAttacking = false;
        agent.isStopped = false; // Resume pathfinding after attack
    }

    // Detect bullet collisions and apply damage
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) // Check if the object is a bullet
        {
            TakeDamage(GetDamage()); // Apply damage to the zombie
            Destroy(collision.gameObject); // Destroy the bullet after collision
        }
    }

    // Reduce the zombie's health by the damage amount
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die(); // Trigger death if health reaches zero
        }
    }

    // Handle zombie death and call the respawn in the spawner
    void Die()
    {
        animator.SetTrigger("Die"); // Trigger the death animation
        agent.isStopped = true; // Stop the zombie's movement
        gameObject.SetActive(false); // Deactivate the zombie
        
    }
}
