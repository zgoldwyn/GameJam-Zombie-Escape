using System.Collections;
using UnityEngine;
using UnityEngine.AI; // For NavMesh

public class ZombieController : MonoBehaviour
{
    public Transform player; // Player's position
    private NavMeshAgent agent; // Zombie's NavMesh agent for pathfinding
    private Animator animator; // To handle animations
    public float attackDistance = 1.5f; // Distance to trigger attack
    public float attackInterval = 1f; // Time between attacks
    public float attackAnimationDuration = 1f; // Duration of the attack animation

    private bool isAttacking = false; // Flag to control attack coroutine
    private float nextAttackTime = 0f; // Tracks time for the next attack

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get NavMeshAgent
        animator = GetComponent<Animator>(); // Get Animator
    }

    void Update()
    {
        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Update the "Distance" parameter in the Animator
        animator.SetFloat("Distance", distanceToPlayer);
        
        // If the zombie is moving (i.e., distance is greater than attack range), set running to true
        if (distanceToPlayer > attackDistance)
        {
            // Only move and chase if not attacking
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
}
