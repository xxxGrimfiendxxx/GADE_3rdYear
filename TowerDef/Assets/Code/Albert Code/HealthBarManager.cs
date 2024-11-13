using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBarPrefab;  // Reference to the health bar prefab (contains SpriteRenderer)
    public Sprite[] levelSprites;  // Array to hold the sprites for each level of tower's health bar

    void Start()
    {
        // Find all towers and add health bars to them
        Tower[] towers = FindObjectsOfType<Tower>();  // Find all tower objects in the scene
        foreach (Tower tower in towers)
        {
            if (tower.GetComponentInChildren<HealthBar>() == null)  // If no health bar is found
            {
                AddHealthBar(tower);
            }
        }
    }

    public void AddHealthBar(Tower tower)
    {
        // Instantiate the health bar and set it as a child of the tower
        GameObject healthBarInstance = Instantiate(healthBarPrefab, tower.transform.position, Quaternion.identity);
        healthBarInstance.transform.SetParent(tower.transform);  // Make health bar a child of the tower
        healthBarInstance.transform.localPosition = new Vector3(0, 1, 0);  // Offset slightly above the tower

        // Set up the health bar
        HealthBar healthBar = healthBarInstance.GetComponent<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(tower.maxHealth);
            healthBar.SetHealth(tower.currentHealth);

            // Set the correct health bar sprite based on tower's current upgrade level
            healthBar.SetHealthBarSprite(tower.currentUpgradeLevel + 1);  // Pass level 1, 2, or 3
        }
    }
}
