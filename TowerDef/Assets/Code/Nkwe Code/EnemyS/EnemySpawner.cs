using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array of enemy prefabs (FastEnemy, HeavyEnemy, etc.)
    public Transform spawnPoint;
    public float spawnInterval = 30f;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        // Randomly select an enemy type to spawn
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        //int spawnRandomIndex = Random.Range(0, spawnPoint.length);//needs more work
        Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);
    }
}

