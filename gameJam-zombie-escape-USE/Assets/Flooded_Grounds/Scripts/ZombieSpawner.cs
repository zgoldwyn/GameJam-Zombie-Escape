using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Reference to the zombie prefab
    [SerializeField] public GameObject[] zombiePool; // Array to hold a pool of zombies
    public Transform player; // Reference to the player for spawning near them
    public int poolSize = 15; // Number of zombies in the pool
    public float spawnHeight = 100f; // Height from which zombies spawn
    public float spawnRadius = 100f; // Radius around the player where zombies can spawn
    public float spawnRate = 3f; // Time between spawn attempts
    public float respawnDelay = 2f; // Delay before respawning zombies after death

    private void Start()
    {
        StartCoroutine(SpawnZombies()); // Begin the spawn cycle
    }

    // Coroutine to manage zombie spawning from the pool
    IEnumerator SpawnZombies()
    {
        while (true)
        {
            // Look for an inactive zombie in the pool
            for (int i = 0; i < zombiePool.Length; i++)
            {
                if (!zombiePool[i].activeInHierarchy)
                {
                    // Get a random position within a circle around the player
                    Vector3 spawnPosition = GetRandomSpawnPosition();

                    // Move the zombie to the spawn position and activate it
                    zombiePool[i].transform.position = spawnPosition;
                    zombiePool[i].SetActive(true); // Activate the zombie
                    
                    // Call a coroutine to activate zombie behavior after it falls
                    StartCoroutine(ActivateZombieAfterFall(zombiePool[i]));

                    break; // Spawn one zombie per cycle
                }
            }

            // Wait before attempting the next spawn
            yield return new WaitForSeconds(spawnRate);
        }
    }

    // Coroutine to handle behavior activation after the zombie falls
    IEnumerator ActivateZombieAfterFall(GameObject zombie)
    {
        NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();
        Rigidbody rb = zombie.GetComponent<Rigidbody>();
        Animator animator = zombie.GetComponent<Animator>();
        
        // Disable the NavMeshAgent during the fall
        agent.enabled = false; 

        // Set the falling trigger in the Animator
        animator.SetBool("isFalling", true);
        
        // Wait for the zombie to hit the ground
        while (zombie.transform.position.y > Terrain.activeTerrain.SampleHeight(zombie.transform.position) + 0.5f)
        {
            yield return null; // Wait for the next frame
        }

        // Once the zombie has hit the ground, deactivate the falling state
        animator.SetBool("isFalling", false);

        // Ensure the zombie is placed on the NavMesh
        Vector3 finalPosition;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(zombie.transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }
        else
        {
            Debug.LogWarning("Zombie not placed on NavMesh!");
            yield break;
        }

        // Set the zombie's position to the valid NavMesh position
        zombie.transform.position = finalPosition;

        // Disable physics after landing
        rb.isKinematic = true;

        // Enable the NavMeshAgent now that the zombie has landed
        agent.enabled = true;

        // Enable the zombie's movement and behavior scripts
        ZombieController zombieController = zombie.GetComponent<ZombieController>();
        zombieController.enabled = true;
    }



    // Generate a random spawn position around the player
    Vector3 GetRandomSpawnPosition()
    {
        // Get a random point inside a circle and place it 10 units above ground
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = new Vector3(player.position.x + randomCircle.x, player.position.y + spawnHeight, player.position.z + randomCircle.y);
        return spawnPosition;
    }
    
}
