using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Reference to the zombie prefab
    [SerializeField] public GameObject[] zombiePool; // Array to hold a pool of zombies
    public Transform player; // Reference to the player for spawning near them
    public int poolSize = 15; // Number of zombies in the pool
    public float spawnHeight = 100f; // Height from which zombies spawn
    public float spawnRadius = 100f; // Radius around the player where zombies can spawn
    public float spawnRate = 5f; // Time between spawn attempts
    public float respawnDelay = 1f; // Delay before respawning zombies after death

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
                    Rigidbody rb = zombiePool[i].GetComponent<Rigidbody>();
                    rb.isKinematic = false; // Allow the zombie to fall

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
        // Wait for 1 second to simulate the fall
        yield return new WaitForSeconds(respawnDelay);

        Rigidbody rb = zombie.GetComponent<Rigidbody>();
        rb.isKinematic = true; // Disable physics once the zombie has landed

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

    // Method to deactivate zombies when they die, called from the ZombieController
    

    // Coroutine to respawn the zombie after death
    
}
