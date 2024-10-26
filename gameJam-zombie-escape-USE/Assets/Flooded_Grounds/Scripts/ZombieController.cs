using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
        if (agent == null || !agent.isOnNavMesh) return; // Ensure the agent is on the NavMesh

        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // Distance to player
    
        if (isFalling) return; // Skip processing while falling
        
        animator.SetFloat("Distance", distanceToPlayer); // Update Animator with distance to player
        
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
            animator.SetBool("IsRunning", false); // Stop running animation
            if (!isAttacking && Time.time >= nextAttackTime)
            {
                StartCoroutine(AttackPlayer()); // Start the attack coroutine if not attacking
            }
        }

        if (health <= 0) Die(); // Handle death when health reaches 0

        // Debug input for testing damage
        if (Input.GetKeyDown(KeyCode.T))
        {
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

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        agent.isStopped = true; 
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackAnimationDuration);

        nextAttackTime = Time.time + attackInterval;
        isAttacking = false;
        agent.isStopped = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) 
        {
            TakeDamage(GetDamage()); 
            collision.gameObject.SetActive(false); 
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }

        if (health <= 0)
        {
            Die(); 
        }
    }

    public void Die()
    {
        // Stop movement and disable the NavMeshAgent
        agent.isStopped = true;
        agent.enabled = false;

        // Trigger death animation, if any
        animator.SetTrigger("Die");

        // Reset health and attack state
        health = 100f;
        isAttacking = false;
        isFalling = true; // Set falling flag to true, to skip processing while respawning

        // Disable the zombie game object
        gameObject.SetActive(false);

        // Notify the spawner to respawn the zombie
        if (spawner != null)
        {
            spawner.Die(gameObject); 
        }
        else
        {
            Debug.LogWarning("No ZombieSpawner found for respawning.");
        }
    }

    public void Respawn(Vector3 newPosition)
    {
        // Reposition the zombie at a valid spawn point
        agent.enabled = false;
        agent.isStopped = true;
        isFalling = true;
        transform.position = newPosition;
        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }
        
        // Re-enable the game object
        gameObject.SetActive(true);

        // Set a delay before enabling the NavMeshAgent again, allowing falling
        new WaitForSeconds(1.5f); // Delay before enabling NavMeshAgent again
        agent.enabled = true;
        isFalling = false;
    }

    
}
