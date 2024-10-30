using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class ZombieController : MonoBehaviour
{
    public Transform player; // Player's position
    public NavMeshAgent agent; // Zombie's NavMesh agent for pathfinding
    private Animator animator; // To handle animations
    public float attackDistance = 1.5f; // Distance to trigger attack
    public float attackInterval = 1f; // Time between attacks
    public float attackAnimationDuration = 1f; // Duration of the attack animation
    public float maxHealth = 100f; // Max health value for the zombie
    private float health; // Current health value for the zombie
    private bool isAttacking = false; // Flag to control attack coroutine
    private float nextAttackTime = 0f; // Tracks time for the next attack
    private ZombieSpawner spawner; // Reference to the ZombieSpawner
    public bool isFalling = false; // Detect if the zombie is falling
    public static int playerHealth = 100;

    // Health bar UI
    public Slider healthBarSlider; // Reference to the slider in the canvas
    Rigidbody rb; 

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>(); // Get NavMeshAgent
        animator = GetComponent<Animator>(); // Get Animator
        spawner = FindObjectOfType<ZombieSpawner>(); // Get the ZombieSpawner in the scene
        rb = gameObject.GetComponent<Rigidbody>();

        // Initialize health
        ResetHealth();

        // Set the max value of the health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth; // Set the maximum value to match health
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
                zombie.TakeDamage(GetZombieDamage()); 
            }
        }
        if (playerHealth <= 0){
            Debug.Log("Health is 0. Game Over");
            SceneManager.LoadScene("GameOver");
            spawner.EndGame();
            
        }
    }

    public int GetZombieDamage()
    {
        return Random.Range(1, 14); // Random damage between 1 and 13
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        agent.isStopped = true; 
        animator.SetTrigger("Attack");
        playerHealth -= Random.Range(1,20);
        Debug.Log(playerHealth.ToString());

        yield return new WaitForSeconds(attackAnimationDuration);

        nextAttackTime = Time.time + attackInterval;
        isAttacking = false;
        agent.isStopped = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) 
        {
            TakeDamage(GetZombieDamage()); 
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
        isAttacking = false;
        //isFalling = true; // Set falling flag to true, to skip processing while respawning

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

    

    private void ResetHealth()
    {
        maxHealth += 5;
        health = maxHealth; // Reset health to max health value
    }


}
