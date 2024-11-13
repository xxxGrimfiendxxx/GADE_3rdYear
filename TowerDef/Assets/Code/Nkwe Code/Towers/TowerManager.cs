using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : MonoBehaviour
{
    public GameObject[] towerPrefabs; // List of tower prefabs to place
    public LayerMask buildableLayer; // Layer that allows tower placement
    public Material validPlacementMaterial; // Material for valid placement preview
    public Material invalidPlacementMaterial; // Material for invalid placement preview
    public int towerCostT;

    private GameObject currentTowerPreview; // Preview of the tower to be placed
    private GameObject currentTowerPrefab; // The tower prefab being placed
    private bool canPlace;
    private int currentTowerIndex = -1; // Index of the currently selected tower

    private UIManager uiManager;

    private void Update()
    {
        if (currentTowerPrefab != null && currentTowerPreview != null)
        {
            // Always update the position of the preview based on mouse movement
            MoveTowerPreview();
            
            if (Input.GetMouseButtonDown(0) && canPlace && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceTower();
            }
        }
    }

    // Selects the tower to be placed from the UI, showing a preview
    public void SelectTower(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Length) return;

        currentTowerIndex = towerIndex;
        currentTowerPrefab = towerPrefabs[towerIndex];

        // Destroy the previous preview if one exists
        if (currentTowerPreview != null)
        {
            Destroy(currentTowerPreview);
        }

        Tower towerComponent = currentTowerPrefab.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerCostT = towerComponent.towerCost; // Set the current tower cost
            towerComponent.isPreview = true; // Set preview mode
        }

        // Instantiate the new preview
        currentTowerPreview = Instantiate(currentTowerPrefab);
        currentTowerPreview.GetComponent<Collider>().enabled = false; // Disable collisions during preview
        SetPreviewMaterial(validPlacementMaterial); // Set valid placement material initially
    }

    // Move the tower preview based on the mouse position
    private void MoveTowerPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check for a valid hit on the terrain or buildable layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildableLayer))
        {
            Vector3 placementPosition = hit.point;
        
            // Adjust Y position based on terrain height (if placing on terrain)
            if (hit.collider.CompareTag("Terrain"))
            {
                placementPosition.y = Terrain.activeTerrain.SampleHeight(placementPosition) + 0.1f; // Raise it slightly to avoid clipping
            }

            currentTowerPreview.transform.position = placementPosition;
            CheckPlacementValidity(hit);
        }
        else
        {
            // If the ray doesn't hit valid terrain, disable placement
            canPlace = false;
            Debug.Log("Raycast did not hit buildable layer.");
        }
    }

    // Checks if the tower can be placed in the current location
    private void CheckPlacementValidity(RaycastHit hit)
    {
        Vector3 position = hit.point;
        
        // Check for any colliders at the target position with a small radius (1f)
        Collider[] colliders = Physics.OverlapSphere(position, 1f);
        
        // Debugging: Output the number of colliders found and what they are
        Debug.Log($"Checking placement validity. Number of colliders found: {colliders.Length}");
        foreach (Collider col in colliders)
        {
            Debug.Log($"Collider found at placement point: {col.gameObject.name}");
        }

        // Ensure the hit point is on valid terrain (e.g., tagged "Terrain") and no overlapping objects exist
        bool hitTerrain = hit.collider.gameObject.CompareTag("Terrain"); // Assuming your terrain has a "Terrain" tag
        canPlace = colliders.Length == 0 && hitTerrain;

        // Debugging: Output whether placement is valid or not
        Debug.Log($"Hit terrain: {hitTerrain}, Can Place: {canPlace}, Currency: {PlayerInfo.currency}, Tower Cost: {towerCostT}");

        // Update the preview material based on placement validity
        if (canPlace && PlayerInfo.currency >= towerCostT)
        {
            SetPreviewMaterial(validPlacementMaterial);
        }
        else
        {
            SetPreviewMaterial(invalidPlacementMaterial);
        }
    }

    // Changes the material of the tower preview
    private void SetPreviewMaterial(Material material)
    {
        Renderer[] renderers = currentTowerPreview.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
    }

    private void PlaceTower()
    {
        if (!canPlace || PlayerInfo.currency < towerCostT) return;

        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        playerInfo.SpendCurrency(towerCostT);

        Vector3 placementPosition = currentTowerPreview.transform.position;
    
        // Adjust Y position based on terrain height to avoid clipping into the ground
        Ray ray = new Ray(placementPosition + Vector3.up * 10, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildableLayer))
        {
            if (hit.collider.CompareTag("BuildableLayer"))
            {
                placementPosition.y = Terrain.activeTerrain.SampleHeight(placementPosition) + 0.1f; // Raise it slightly
            }
        }

        // Instantiate the tower at the correct position
        GameObject newTower = Instantiate(currentTowerPrefab, placementPosition, Quaternion.identity);
        newTower.GetComponent<Collider>().enabled = true;

        Tower towerComponent = newTower.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerComponent.isPreview = false; // Tower is no longer in preview mode
        }

        Destroy(currentTowerPreview);
        currentTowerPrefab = null;
        currentTowerPreview = null;

        Debug.Log("Tower placed successfully.");
    }
}


