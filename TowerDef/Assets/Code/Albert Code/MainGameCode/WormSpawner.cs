using UnityEngine;

public class WormSpawner : MonoBehaviour
{
    public GameObject wormPrefab; // Worm prefab
    public Transform[] markerA; // Markers for worm A
    public Transform[] markerB; // Markers for worm B
    public Transform[] markerC; // Markers for worm C
    public Transform center; // The center position
    public Color wormColor; // Color for the worms

    private void Start()
    {
        SpawnWorm(markerA, "WormA");
        SpawnWorm(markerB, "WormB");
        SpawnWorm(markerC, "WormC");
    }

    private void SpawnWorm(Transform[] markers, string name)
    {
        GameObject worm = Instantiate(wormPrefab, markers[0].position, Quaternion.identity);
        worm.name = name;
        WormController controller = worm.GetComponent<WormController>();
        controller.markers = markers;
        controller.center = center;
        controller.wormColor = wormColor;
    }
}
