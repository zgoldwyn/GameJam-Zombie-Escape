using System.Collections;
using UnityEngine;
using UnityEngine.AI; // For NavMesh

public class ZombieController : MonoBehaviour
{
    public Transform player; // Player's position
    private NavMeshAgent agent; // Zombie's NavMesh agent for pathfinding
    private Animator animator; // To handle animations
    public float attackDistance = 1.5f; // Distance to trigger attack
    public float spawnRate = 5f; // Time between spawns

    private bool isAttacking = false;

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
        animator.SetBool("IsRunning", true);
        agent.SetDestination(player.position); // Keep moving towards player
    }
    else
    {
        animator.SetBool("IsRunning", false); // Stop running when close to attack
        StartCoroutine(AttackPlayer()); // Trigger attack when in range
    }
}


    IEnumerator AttackPlayer()
    {
        isAttacking = true; // Stop the zombie from moving
        agent.isStopped = true; // Stop pathfinding
        animator.SetTrigger("Attack"); // Play attack animation
        yield return new WaitForSeconds(2f); // Wait for animation to play
        agent.isStopped = false; // Resume pathfinding
        isAttacking = false;
    }
}
