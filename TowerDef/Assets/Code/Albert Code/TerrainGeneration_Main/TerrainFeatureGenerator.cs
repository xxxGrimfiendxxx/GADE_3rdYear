using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFeatureGenerator : MonoBehaviour
{
    public GameObject boidPrefab;
    public Terrain terrain;
    public int numBoids = 100;

    private void Start()
    {
        for (int i = 0; i < numBoids; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(0, terrain.terrainData.size.x),
                0,
                Random.Range(0, terrain.terrainData.size.z)
            );

            position.y = terrain.SampleHeight(position);
            Instantiate(boidPrefab, position, Quaternion.identity);
        }
    }
}
