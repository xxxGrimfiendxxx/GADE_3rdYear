using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [System.Serializable]
    public class BoidTypeInfo
    {
        public GameObject boidPrefab;
        public Boid.BoidType boidType;
        public int count;
        public float minDistanceBetweenTypes; // Minimum distance to maintain between specified types
    }

    [Header("Boid Settings")]
    public Texture2D boidTexture; // Initial boid texture

    public List<BoidTypeInfo> boidTypes;
    public Terrain terrain;
    public float duration = 30.0f;

    public List<GameObject> existingBoids = new List<GameObject>();
    public List<GameObject> startingBoids = new List<GameObject>();

    public Texture2D finalBoidTexture; // Texture captured from terrain after 22 seconds
    public float captureDelay = 19f; // Time after which to capture the texture

    public TextureManager textureManager; // Reference to TextureManager

    private void Start()
    {
        // Initialize boidTexture with terrain size
        boidTexture = new Texture2D(terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

        StartCoroutine(InitializeBoids());
    }

    public IEnumerator InitializeBoids()
    {
        Debug.Log("Initializing boids...");

        // Initialize boids based on BoidTypeInfo
        foreach (var boidTypeInfo in boidTypes)
        {
            for (int i = 0; i < boidTypeInfo.count; i++)
            {
                Vector3 position = GetRandomPosition();

                // Ensure the position is valid and meets the distance criteria
                while (!IsValidPosition(position, boidTypeInfo.boidType))
                {
                    position = GetRandomPosition();
                }

                position.y = terrain.SampleHeight(position);
                GameObject boidInstance = Instantiate(boidTypeInfo.boidPrefab, position, Quaternion.identity);
                existingBoids.Add(boidInstance);

                // Add starting boid to the list
                if (i == 0) // Adjust this condition if needed
                {
                    startingBoids.Add(boidInstance);
                }
            }
        }

        // Simulate some delay for initialization (2 seconds)
        yield return new WaitForSeconds(2f);

        // Generate boid texture and log texture details
        boidTexture = GenerateBoidTexture();
        if (boidTexture != null)
        {
            Debug.Log($"Boid Texture Size: {boidTexture.width}x{boidTexture.height}");
            Debug.Log($"First Pixel Color: {boidTexture.GetPixel(0, 0)}");
            if (textureManager != null)
            {
                textureManager.StoreBoidTexture(boidTexture); // Assuming a method exists to store the boid texture
            }
        }
        else
        {
            Debug.LogError("Failed to generate boid texture.");
        }

        // Wait for the capture delay before capturing the terrain texture
        yield return new WaitForSeconds(captureDelay - 2f); // Subtracting already waited time

        // Continue with the rest of the boid management
        yield return new WaitForSeconds(duration - captureDelay);

        // Destroy all boids including the original ones
        foreach (GameObject boid in existingBoids)
        {
            Destroy(boid);
        }
        existingBoids.Clear();

        foreach (GameObject boid in startingBoids)
        {
            Destroy(boid);
        }
        startingBoids.Clear();
    }

    private Texture2D GenerateBoidTexture()
    {
        Texture2D texture = new Texture2D(121, 121);
        // Populate texture with boid data
        Debug.Log("Generating boid texture...");
        // Assume some texture generation logic here
        return texture;
    }

    public Texture2D GetBoidTexture()
    {
        return boidTexture;
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(-65, terrain.terrainData.size.x),
            0,
            Random.Range(-65, terrain.terrainData.size.z)
        );
    }

    private bool IsValidPosition(Vector3 position, Boid.BoidType type)
    {
        foreach (var boidTypeInfo in boidTypes)
        {
            if (boidTypeInfo.boidType == type)
            {
                continue;
            }

            foreach (Boid boidScript in FindObjectsOfType<Boid>())
            {
                if (boidScript.boidType != type)
                {
                    if (Vector3.Distance(position, boidScript.transform.position) < boidTypeInfo.minDistanceBetweenTypes)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
