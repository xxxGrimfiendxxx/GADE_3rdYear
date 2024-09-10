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
    private bool canPlace = false;
    private int currentTowerIndex = -1; // Index of the currently selected tower

    private UIManager uiManager;

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
        Tower towerComponent = currentTowerPrefab.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerCostT = towerComponent.towerCost; // Set the current tower cost
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildableLayer))
        {
            currentTowerPreview.transform.position = hit.point;
            CheckPlacementValidity(hit.point);
        }
    }

    // Checks if the tower can be placed in the current location
    private void CheckPlacementValidity(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 1f); // Check for overlaps at the placement point
        canPlace = colliders.Length == 0; // Allow placement if no other object is at the location

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
            Debug.Log("Cannot place tower. Invalid location or not enough currency.");
            return;
        }

        // Deduct currency
        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        playerInfo.SpendCurrency(towerCostT);

        // Instantiate the real tower
        GameObject newTower = Instantiate(currentTowerPrefab, currentTowerPreview.transform.position, Quaternion.identity);
        newTower.GetComponent<Collider>().enabled = true; // Enable collisions for the real tower

        Destroy(currentTowerPreview);
        currentTowerPrefab = null;
        currentTowerPreview = null;
    }
}
