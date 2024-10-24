using System.Collections; // For IEnumerator
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // For handling UI elements

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
    public bool isFalling = false; // Detect if the zombie is falling

    // Health bar UI
    public Slider healthBarSlider; // Reference to the slider in the canvas

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get NavMeshAgent
        animator = GetComponent<Animator>(); // Get Animator
        spawner = FindObjectOfType<ZombieSpawner>(); // Get the ZombieSpawner in the scene

        // Set the max value of the health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = health; // Set the maximum value to match health
            healthBarSlider.value = health; // Set the current value
        }
    }

    void Update()
    {
        // Check if the agent is valid and is on the NavMesh
        if (agent == null || !agent.isOnNavMesh)
        {
            return; // If not, don't proceed further to avoid errors
        }

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    
        // If the zombie is falling, do not process further
        if (isFalling) return;
        
        // Update Animator with distance to player
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

        // Listen for the 'T' key press and apply damage to all zombies
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Find all zombies in the scene
            ZombieController[] zombies = FindObjectsOfType<ZombieController>();
            foreach (ZombieController zombie in zombies)
            {
                zombie.TakeDamage(GetDamage()); 
            }
        }
    }

    public int GetDamage()
    {
        return Random.Range(1, 14); // Random damage between 1 and 13
    }

    // Coroutine for handling attacking behavior
    IEnumerator AttackPlayer()
    {
        isAttacking = true; // Set attacking flag
        agent.isStopped = true; // Stop the agent from moving while attacking
        animator.SetTrigger("Attack"); // Trigger attack animation

        // Wait for the attack animation to finish playing
        yield return new WaitForSeconds(attackAnimationDuration);

        // Set the next attack time
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
            collision.gameObject.SetActive(false); // Destroy the bullet after collision
        }
    }

    // Reduce the zombie's health by the damage amount
    public void TakeDamage(int damage)
    {
        health -= damage;

        // Update the health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.value = health; // Update the slider value
        }

        if (health <= 0)
        {
            Die(); // Trigger death if health reaches zero
        }
    }

    // Handle zombie death and call the respawn in the spawner
    public void Die()
    {
        // Stop the zombie's movement and behavior
        agent.isStopped = true;
        agent.enabled = false; // Disable the NavMeshAgent to prevent unwanted movement

        // Reset health and behavior states
        health = 100f;
        isAttacking = false;
        isFalling = true;

        // Deactivate the zombie
        gameObject.SetActive(false);

        // Call the ZombieSpawner's Die method
        if (spawner != null)
        {
            spawner.Die(gameObject);
        }
        else
        {
            Debug.LogWarning("ZombieSpawner not found! Ensure there is a ZombieSpawner in the scene.");
        }
    }
}
