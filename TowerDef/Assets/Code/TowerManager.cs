using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    // Reference to the main tower
    public Tower mainTower;

    // List of all towers in the game
    public List<Tower> towers = new List<Tower>();

    // Reference to the UI Manager to handle game over UI
   // public UIManager uiManager;

    void Start()
    {
        if (mainTower == null)
        {
            Debug.LogError("Main Tower is not assigned!");
            return;
        }

        // Add the main tower to the tower list
        towers.Add(mainTower);

        // Subscribe to the main tower's OnDeath event
        mainTower.OnDeath += GameOver;
    }

    // Function to add new tower types seamlessly
    public void AddTower(Tower towerPrefab, Vector3 position)
    {
        Tower newTower = Instantiate(towerPrefab, position, Quaternion.identity);
        towers.Add(newTower);
    }

    // Function to remove a tower (e.g., if destroyed)
    public void RemoveTower(Tower tower)
    {
        towers.Remove(tower);
    }

    // Game over when the main tower dies
    private void GameOver()
    {
        Debug.Log("Game Over! The main tower has been destroyed.");
        // Show game over UI or trigger any end-game logic
      //  if (uiManager != null)
        {
           // uiManager.ShowGameOverScreen();
        }
    }
}

