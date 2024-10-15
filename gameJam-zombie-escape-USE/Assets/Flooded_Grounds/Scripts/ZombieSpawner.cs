using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Reference to the zombie prefab
    public Transform[] spawnPoints; // Array of spawn points
    public float spawnRate = 5f; // Time between spawns

    void Start()
    {
        StartCoroutine(SpawnZombies()); // Begin spawning
    }

    IEnumerator SpawnZombies()
    {
        while (true)
        {
            // Select a random spawn point
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            // Spawn the zombie at the chosen location
            Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

            // Wait before the next spawn
            yield return new WaitForSeconds(spawnRate);
        }
    }
}
