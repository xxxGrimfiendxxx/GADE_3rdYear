using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CubePathGenerator))]
public class CubePathGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CubePathGenerator script = (CubePathGenerator)target;
        if (GUILayout.Button("Generate Path"))
        {
            script.GeneratePath();
        }
        if (GUILayout.Button("Clear Path"))
        {
            script.ClearPath();
        }
    }
}


public class CubePathGenerator : MonoBehaviour
{
    [Header("Cube Settings")]
    [SerializeField] private GameObject cubePrefab; // Prefab of the cube to be used for the path
    [SerializeField] private float cubeSpacing = 1f; // Spacing between the cubes
    [SerializeField] private Vector3 cubeScale = new Vector3(1f, 1f, 1f); // Scale of the cubes

    [Header("Path Points")]
    [SerializeField] private Transform[] pathPoints; // Marker objects that define the path

    private void OnValidate()
    {
        // Automatically update path points from child objects
        pathPoints = GetComponentsInChildren<Transform>();
    }

    public void GeneratePath()
    {
        ClearPath();

        if (cubePrefab == null || pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError("Cube prefab or path points are not set correctly.");
            return;
        }

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            Vector3 startPoint = pathPoints[i].position;
            Vector3 endPoint = pathPoints[i + 1].position;
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

    public void ClearPath()
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
