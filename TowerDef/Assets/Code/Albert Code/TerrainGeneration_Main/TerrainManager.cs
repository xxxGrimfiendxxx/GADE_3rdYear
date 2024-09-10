using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain; // Reference to the terrain
    public TerrainLayer[] terrainLayers; // Assign in the Inspector
    public RenderTexture renderTexture; // Optional: For real-time display on a plane
    public float paintRadius = 1.0f; // Radius of the paint effect
    public Color centralColor = Color.green; // Color to paint around the center

    private Texture2D persistentTexture; // Persistent texture to accumulate paint
    private int textureResolution = 121; // Example resolution; adjust as needed
    private Vector3 terrainCenter;

    private float minRadius = 1f; // Minimum radius
    private float maxRadius = 6f; // Maximum radius
    private float radiusChangeSpeed = 1f; // Speed of radius change

    private void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return;
        }

        terrainCenter = new Vector3(terrain.terrainData.size.x / 2, 0, terrain.terrainData.size.z / 2);

        // Initialize the persistent texture
        persistentTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
        ClearTexture(persistentTexture, Color.blue);
        ApplyTextureToTerrain();

        // Paint the central area
        PaintCentralArea();

        StartCoroutine(SaveTextureAfterDelay("MyPaintedTerrain", 20f));

        // Start the coroutine to adjust the radius
        StartCoroutine(AdjustRadius());
    }

    // Coroutine to randomly grow and shrink the radius
    private IEnumerator AdjustRadius()
    {
        while (true)
        {
            // Randomly adjust the radius within the defined range
            paintRadius = Mathf.Lerp(minRadius, maxRadius, Mathf.PingPong(Time.time * radiusChangeSpeed, 1));
            yield return null;
        }
    }

    // Clear the texture with a base color
    private void ClearTexture(Texture2D texture, Color color)
    {
        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    // Paint on the persistent texture based on the boid's position
    public void Paint(Vector3 worldPosition, Color paintColor)
    {
        // Convert world position to texture coordinates
        Vector3 terrainPosition = terrain.transform.InverseTransformPoint(worldPosition);
        float x = Mathf.InverseLerp(0, terrain.terrainData.size.x, terrainPosition.x) * persistentTexture.width;
        float z = Mathf.InverseLerp(0, terrain.terrainData.size.z, terrainPosition.z) * persistentTexture.height;

        int radius = Mathf.CeilToInt(paintRadius);

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dz = -radius; dz <= radius; dz++)
            {
                int pixelX = Mathf.Clamp((int)x + dx, 0, persistentTexture.width - 1);
                int pixelZ = Mathf.Clamp((int)z + dz, 0, persistentTexture.height - 1);

                // Calculate the distance from the center
                float dist = Vector2.Distance(new Vector2(x, z), new Vector2(pixelX, pixelZ));

                // If within the paint radius, paint the pixel
                if (dist <= paintRadius)
                {
                    persistentTexture.SetPixel(pixelX, pixelZ, paintColor);
                }
            }
        }

        // Apply the changes to the texture
        persistentTexture.Apply();

        UpdateTerrainTexture();

        // Optionally update the render texture for real-time display
        if (renderTexture != null)
        {
            UpdateRenderTexture();
        }
    }

    // Paint a central area with a specific color
    public void PaintCentralArea()
    {
        float radius = 15.0f;
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 center = new Vector3(terrainSize.x / 2, 0, terrainSize.z / 2);

        for (int x = 0; x < persistentTexture.width; x++)
        {
            for (int z = 0; z < persistentTexture.height; z++)
            {
                float tx = Mathf.InverseLerp(0, persistentTexture.width, x);
                float tz = Mathf.InverseLerp(0, persistentTexture.height, z);

                Vector3 pixelWorldPos = new Vector3(
                    Mathf.Lerp(0, terrainSize.x, tx),
                    0,
                    Mathf.Lerp(0, terrainSize.z, tz)
                );

                if (Vector3.Distance(pixelWorldPos, center) < radius)
                {
                    persistentTexture.SetPixel(x, z, centralColor);
                }
            }
        }

        persistentTexture.Apply();
        UpdateTerrainTexture();
    }

    public Color GetColorAtPosition(Vector3 position)
    {
        // Convert world position to terrain coordinates
        float x = Mathf.InverseLerp(0, terrain.terrainData.size.x, position.x) * terrain.terrainData.alphamapWidth;
        float z = Mathf.InverseLerp(0, terrain.terrainData.size.z, position.z) * terrain.terrainData.alphamapHeight;

        // Get the terrain texture at the coordinates
        Texture2D texture = terrain.terrainData.terrainLayers[0].diffuseTexture as Texture2D;
        if (texture != null)
        {
            Color color = texture.GetPixel((int)x, (int)z);
            return color;
        }

        return Color.clear;
    }

    private void UpdateTerrainTexture()
    {
        TerrainData terrainData = terrain.terrainData;

        // Create a new TerrainLayer or update existing layers
        TerrainLayer[] terrainLayers = new TerrainLayer[terrainData.terrainLayers.Length];

        for (int i = 0; i < terrainData.terrainLayers.Length; i++)
        {
            terrainLayers[i] = terrainData.terrainLayers[i];
        }

        // Add or replace the texture in terrainLayers
        TerrainLayer newLayer = new TerrainLayer
        {
            diffuseTexture = persistentTexture // Assign the painted texture here
        };

        terrainLayers[0] = newLayer; // Replace the first layer or choose the appropriate index
        terrainData.terrainLayers = terrainLayers;

        // Adjust tileSize to match terrain size
        if (terrainLayers.Length > 0)
        {
            TerrainLayer layer = terrainLayers[0];
            layer.tileSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
        }
    }

    private void ApplyTextureToTerrain()
    {
        // Apply the persistent texture to the terrain material or any other surface
        terrain.materialTemplate.mainTexture = persistentTexture;

        if (terrain.materialTemplate != null)
        {
            terrain.materialTemplate.mainTexture = persistentTexture;
        }
        else
        {
            Debug.LogError("Terrain material is not assigned.");
        }
    }

    private void UpdateRenderTexture()
    {
        RenderTexture.active = renderTexture;
        Graphics.Blit(persistentTexture, renderTexture);
        RenderTexture.active = null;
    }

    private IEnumerator SaveTextureAfterDelay(string fileName, float delay)
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(delay);

        // Save the texture
        SaveTexture(fileName);
    }

    // Save the texture to a PNG file
    public void SaveTexture(string fileName)
    {
        // Apply texture changes before saving
        persistentTexture.Apply();

        // Set alpha channel to opaque for the entire texture if needed
        Color[] pixels = persistentTexture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].a = 1.0f; // Set alpha to 1 (fully opaque)
        }
        persistentTexture.SetPixels(pixels);
        persistentTexture.Apply();

        // Convert the Texture2D to a PNG
        byte[] bytes = persistentTexture.EncodeToPNG();

        // Save the PNG in the "SavedTextures" folder within the project's Assets folder
        string path = Path.Combine(Application.dataPath, "SavedTextures");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, fileName + ".png");

        try
        {
            File.WriteAllBytes(path, bytes);
            Debug.Log("Texture saved to: " + path);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save texture: " + ex.Message);
        }
    }
}
