using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManagerTest : MonoBehaviour
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

    private void Update()
    {
        if (currentTowerPrefab != null && currentTowerPreview != null)
        {
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

        if (currentTowerPreview != null)
        {
            Destroy(currentTowerPreview);
        }

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
                placementPosition.y = Terrain.activeTerrain.SampleHeight(placementPosition) + 0.1f; // Raise it slightly
            }

            currentTowerPreview.transform.position = placementPosition;
            CheckPlacementValidity(hit);
        }
        else
        {
            // If the ray doesn't hit valid terrain, disable placement
            canPlace = false;
            SetPreviewMaterial(invalidPlacementMaterial);
            Debug.Log("Raycast did not hit a valid placement area.");
        }
    }

    // Checks if the tower can be placed in the current location
    private void CheckPlacementValidity(RaycastHit hit)
    {
        // Simple check for overlapping objects
        Collider[] colliders = Physics.OverlapSphere(hit.point, 1f);
        canPlace = colliders.Length == 0;

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

    // Places the tower if valid
    private void PlaceTower()
    {
        if (!canPlace || PlayerInfo.currency < towerCostT)
        {
            Debug.Log("Cannot place tower: Invalid location or insufficient currency.");
            return;
        }

        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        playerInfo.SpendCurrency(towerCostT);

        Vector3 placementPosition = currentTowerPreview.transform.position;

        // Adjust Y position based on terrain height to avoid clipping into the ground
        if (Physics.Raycast(new Ray(placementPosition + Vector3.up * 10, Vector3.down), out RaycastHit hit, Mathf.Infinity, buildableLayer))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                placementPosition.y = Terrain.activeTerrain.SampleHeight(placementPosition) + 0.1f;
            }
        }

        // Instantiate the tower at the correct position
        GameObject newTower = Instantiate(currentTowerPrefab, placementPosition, Quaternion.identity);
        newTower.GetComponent<Collider>().enabled = true;

        Destroy(currentTowerPreview);
        currentTowerPrefab = null;
        currentTowerPreview = null;

        Debug.Log("Tower placed successfully.");
    }
}

