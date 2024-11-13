using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManagerTest : MonoBehaviour
{
    public GameObject[] towerPrefabs; // List of tower prefabs to place
    public int towerCostT; // The current cost of the selected tower

    private GameObject currentTowerPreview; // Preview of the tower to be placed
    private GameObject currentTowerPrefab; // The tower prefab being placed
    private int currentTowerIndex = -1; // Index of the currently selected tower

    private void Update()
    {
        if (currentTowerPrefab != null && currentTowerPreview != null)
        {
            MoveTowerPreview();
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                TryPlaceTower();
            }
        }
    }

    // Selects the tower to be placed from the UI, showing a preview
    public void SelectTower(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Length) return;

        currentTowerIndex = towerIndex;
        currentTowerPrefab = towerPrefabs[towerIndex];

        if (currentTowerPreview != null)
        {
            Destroy(currentTowerPreview);
        }

        // Set the tower cost from the tower component
        Tower towerComponent = currentTowerPrefab.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerCostT = towerComponent.towerCost; // Get the cost of the selected tower
        }

        currentTowerPreview = Instantiate(currentTowerPrefab);
        currentTowerPreview.GetComponent<Collider>().enabled = false; // Disable collisions during preview
    }

    // Move the tower preview based on the mouse position
    private void MoveTowerPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            currentTowerPreview.transform.position = hit.point;
        }
    }

    // Tries to place the tower with a currency check
    private void TryPlaceTower()
    {
        if (PlayerInfo.currency < towerCostT) // Check if the player has enough currency
        {
            Debug.Log("Not enough currency to place this tower!");
            return;
        }

        // Deduct currency
        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        playerInfo.SpendCurrency(towerCostT);

        // Instantiate the actual tower with the correct rotation from the prefab
        GameObject newTower = Instantiate(currentTowerPrefab, currentTowerPreview.transform.position, currentTowerPrefab.transform.rotation);
        newTower.GetComponent<Collider>().enabled = true; // Enable collider for the actual tower

        // Destroy the preview and reset the current tower
        Destroy(currentTowerPreview);
        currentTowerPrefab = null;
        currentTowerPreview = null;
    }
}