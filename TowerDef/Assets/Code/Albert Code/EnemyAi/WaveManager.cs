using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Tower Settings")]
    public TMP_Text towerPointsText; // Reference to the tower points textbox
    public float safePeriodDuration = 20f; // Duration of the safe period where players can build towers
    public float waveCooldownDuration = 10f; // Time to wait between waves

    [Header("Enemy Settings")]
    public List<GameObject> enemyPrefabs; // List of enemy prefabs to spawn
    public List<int> enemyPoints; // List of corresponding points for each enemy
    public GameObject spawnArea; // Reference to the spawn area GameObject
    public float spawnHeight = 1.0f; // Height at which enemies should spawn

    [Header("Wave Settings")]
    public TMP_Text waveCounterText; // Text box displaying the wave count
    public int gameStage = 0; // Game stage modifier
    public int difficulty = 0; // Difficulty modifier
    private int waveNumber = 0; // Current wave number
    private bool isSafePeriod = true; // Track whether it's safe to build

    private int towerPoints; // Points available based on towers
    private List<GameObject> enemiesInWave = new List<GameObject>(); // Enemies to spawn in the current wave
    private List<GameObject> activeEnemies = new List<GameObject>(); // List of active enemies in the scene

    private void Start()
    {
        waveNumber = 0;
        waveCounterText.text = "Wave: " + waveNumber;
        StartCoroutine(SafePeriod());
    }

    private void Update()
    {
        // Clean up the active enemies list, removing any that are no longer present
        activeEnemies.RemoveAll(enemy => enemy == null);

        // If all enemies are defeated and there is no active wave, start the safe period again
        if (activeEnemies.Count == 0 && enemiesInWave.Count == 0 && !isSafePeriod)
        {
            StartCoroutine(SafePeriod());
        }
    }

    // Starts the safe period, allowing the player to build towers
    IEnumerator SafePeriod()
    {
        isSafePeriod = true;
        Debug.Log("Safe period started. You can build towers!");
        yield return new WaitForSeconds(safePeriodDuration);

        // Safe period ends, calculate wave points and spawn enemies
        isSafePeriod = false;
        StartNextWave();
    }

    // Starts the next wave and calculates how many enemies to spawn
    void StartNextWave()
    {
        waveNumber++;
        waveCounterText.text = "Wave: " + waveNumber;

        // Get the tower points from the TMP textbox
        if (int.TryParse(towerPointsText.text, out towerPoints))
        {
            towerPoints += (gameStage + difficulty); // Modify based on game stage and difficulty
            CalculateEnemiesToSpawn();
            SpawnWave();
        }
        else
        {
            Debug.LogError("Failed to read tower points from text box.");
        }
    }

    // Calculates the number of each enemy to spawn based on tower points, with weighted probabilities
    void CalculateEnemiesToSpawn()
    {
        int availablePoints = towerPoints;
        enemiesInWave.Clear(); // Reset the list for the new wave

        // Probabilities (weights) for each enemy class
        float[] spawnChances = { 0.5f, 0.3f, 0.2f }; // 50% for enemy 1, 30% for enemy 2, 20% for enemy 3

        // Loop until all tower points are spent
        while (availablePoints > 0)
        {
            int chosenEnemyIndex = GetWeightedRandomEnemy(spawnChances);
            int enemyCost = enemyPoints[chosenEnemyIndex];

            // Check if enough points remain to "buy" this enemy
            if (availablePoints >= enemyCost)
            {
                availablePoints -= enemyCost;
                enemiesInWave.Add(enemyPrefabs[chosenEnemyIndex]);
            }
            else
            {
                // If no more enemies can be bought, stop the loop
                break;
            }
        }

        Debug.Log("Wave " + waveNumber + " will spawn " + enemiesInWave.Count + " enemies.");
    }

    // Get a random enemy index based on weighted probabilities
    int GetWeightedRandomEnemy(float[] weights)
    {
        float total = 0;
        foreach (float weight in weights)
        {
            total += weight;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < weights.Length; i++)
        {
            if (randomPoint < weights[i])
            {
                return i;
            }
            else
            {
                randomPoint -= weights[i];
            }
        }

        return weights.Length - 1; // Fallback to last enemy type if something goes wrong
    }

    // Spawns the calculated enemies
    void SpawnWave()
    {
        foreach (GameObject enemyPrefab in enemiesInWave)
        {
            Vector3 spawnPosition = GetRandomPositionWithinBounds(spawnArea);
            spawnPosition.y = spawnHeight; // Set the spawn height
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            activeEnemies.Add(spawnedEnemy); // Track the active enemies
        }

        enemiesInWave.Clear(); // Clear the wave list after spawning
    }

    // Get a random position within the spawn area bounds
    Vector3 GetRandomPositionWithinBounds(GameObject area)
    {
        Bounds bounds = area.GetComponent<Collider>().bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, bounds.min.y + spawnHeight, randomZ); // Set the Y based on spawn height
    }
}
