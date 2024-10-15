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
        if (!isAttacking)
        {
            // Move towards the player
            agent.SetDestination(player.position);

            // Check distance to player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackDistance)
            {
                StartCoroutine(AttackPlayer()); // Trigger attack animation
            }
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
