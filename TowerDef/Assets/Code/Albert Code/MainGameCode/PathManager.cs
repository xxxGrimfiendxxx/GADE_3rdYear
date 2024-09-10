using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathwayManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathManager script = (PathManager)target;
        if (GUILayout.Button("Generate Paths"))
        {
            script.GeneratePaths();
        }
        if (GUILayout.Button("Clear Paths"))
        {
            script.ClearPaths();
        }
    }
}
#endif

public class PathManager : MonoBehaviour
{
    [Header("Marker Settings")]
    public GameObject markerPrefab; // Prefab for the markers
    public int markersPerSector = 10; // Number of markers to spawn per sector
    public float moveSpeed = 5f; // Speed at which markers move towards the center

    [Header("Cube Path Settings")]
    [SerializeField] private GameObject cubePrefab; // Prefab of the cube to be used for the path
    [SerializeField] private float cubeSpacing = 1f; // Spacing between the cubes
    [SerializeField] private Vector3 cubeScale = new Vector3(1f, 1f, 1f); // Scale of the cubes

    [Header("Path Generation")]
    public GameObject giantCube; // The giant cube to be split into sectors
    public float cubeSize = 30f; // Size of the cube area

    private Vector3[] sectorCenters;
    private Transform[] markers;

    void Start()
    {
        if (giantCube != null)
        {
            InitializeSectorsAndMarkers();
            GeneratePaths(); // Generate paths after markers have spawned
        }
        else
        {
            Debug.LogError("Giant Cube is not assigned.");
        }
    }

    private void InitializeSectorsAndMarkers()
    {
        // Divide the giant cube into 3 sectors
        sectorCenters = new Vector3[3];
        float sectorSize = cubeSize / 3f;

        for (int i = 0; i < 3; i++)
        {
            // Calculate sector centers by adjusting Z position
            sectorCenters[i] = giantCube.transform.position + new Vector3(0, 0, (i - 1) * sectorSize);
            SpawnMarkersInSector(sectorCenters[i], sectorSize);
        }
    }

    private void SpawnMarkersInSector(Vector3 sectorCenter, float sectorSize)
    {
        for (int i = 0; i < markersPerSector; i++)
        {
            // Random position within the sector
            Vector3 randomPosition = new Vector3(
                Random.Range(sectorCenter.x - sectorSize / 2f, sectorCenter.x + sectorSize / 2f),
                0, // Adjust Y later
                Random.Range(sectorCenter.z - sectorSize / 2f, sectorCenter.z + sectorSize / 2f)
            );

            // Set Y position to match terrain height
            randomPosition.y = Terrain.activeTerrain.SampleHeight(randomPosition);

            // Instantiate marker and initialize movement
            GameObject marker = Instantiate(markerPrefab, randomPosition, Quaternion.identity);
            marker.GetComponent<MarkerMover>().Initialize(giantCube.transform.position, moveSpeed);
        }
    }

    public void GeneratePaths()
    {
        ClearPaths(); // Clear existing paths before generating new ones

        // Ensure markers are initialized
        markers = GetComponentsInChildren<Transform>();
        if (cubePrefab == null || markers.Length < 2)
        {
            Debug.LogError("Cube prefab or markers are not set correctly.");
            return;
        }

        // Generate paths from markers
        for (int i = 0; i < markers.Length - 1; i++)
        {
            Vector3 startPoint = markers[i].position;
            Vector3 endPoint = markers[i + 1].position;
            GenerateCubesBetweenPoints(startPoint, endPoint);
        }
    }

    private void GenerateCubesBetweenPoints(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        int cubeCount = Mathf.FloorToInt(distance / cubeSpacing);

        for (int j = 0; j <= cubeCount; j++)
        {
            Vector3 position = start + direction * j * cubeSpacing;
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, transform);
            cube.transform.localScale = cubeScale;
        }
    }

    public void ClearPaths()
    {
        // Removes all cube children from the parent object
        foreach (Transform child in transform)
        {
            if (child != transform) // Avoid destroying the parent itself
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}

public class MarkerMover : MonoBehaviour
{
    private Vector3 targetCenter;
    private float moveSpeed;

    // Initialize the marker with the target center and move speed
    public void Initialize(Vector3 center, float speed)
    {
        targetCenter = center;
        moveSpeed = speed;
    }

    void Update()
    {
        // Move marker towards the center by the specified step
        Vector3 direction = (targetCenter - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Adjust the Y position to stay on the terrain
        Vector3 position = transform.position;
        position.y = Terrain.activeTerrain.SampleHeight(position);
        transform.position = position;
    }
}
