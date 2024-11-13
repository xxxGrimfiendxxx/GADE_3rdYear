using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class TerrainManagerElectricBogalooA : MonoBehaviour
{
    #region Header Variables

    [Header("Terrain and Terrain Layers")]
    public Terrain terrain; // Assign the terrain object in the inspector
    public TerrainLayer heightMapLayer; // Assign the heightmap layer
    public TerrainLayer perlinNoiseLayer; // Assign the Perlin noise layer
    public TerrainLayer pathTextureLayer; // Assign the path texture layer

    [Header("Painting and Color Transition")]
    public float paintRadius = 1.0f;
    public Color initialColor = Color.green;
    public Color targetColor = Color.red;
    public float transitionDuration = 10.0f;


    [Header("Texture Resolution")]
    public int textureResolution = 121;

    [Header("Perlin Noise Settings")]
    public float scaleMin = 10f;
    public float scaleMax = 20f;
    public float heightScaleMin = 1f;
    public float heightScaleMax = 5f;
    public float borderWidth = 3f; // Width of the border area
    public float islandRadiusMin = 50f; // Minimum radius of the island effect
    public float islandRadiusMax = 100f; // Maximum radius of the island effect
    public float offsetXMin = 0f; // Minimum X offset for the noise
    public float offsetXMax = 10f; // Maximum X offset for the noise
    public float offsetYMin = 0f; // Minimum Y offset for the noise
    public float offsetYMax = 10f; // Maximum Y offset for the noise

    [Header("Layer Colors")]
    public Color LowestLayer = new Color(0.0f, 0.75f, 0.5f);
    public Color midLayer = new Color(0.0f, 0.75f, 0.0f);
    public Color TopLayer = new Color(0.5f, 1.0f, 0.5f);

    [Header("Target Colors for Transition")]
    public Color targetLowestLayer = new Color(0.2f, 0.5f, 1.0f); // New color for transition
    public Color targetMidLayer = new Color(0.2f, 0.6f, 0.3f);     // New color for transition
    public Color targetTopLayer = new Color(0.7f, 0.8f, 0.2f);     // New color for transition

    #endregion

    #region Private Variables

    private Texture2D perlinTexture;
    private Texture2D heightMapTexture;
    private Texture2D pathTexture;
    private List<Vector2Int> startPositions;
    private Vector2Int centerPosition;

    private const float threshold1 = 0.1f; // Define thresholds
    private const float threshold2 = 0.2f;
    private int seed;
    #endregion

    #region Initialization

    private void Start()
    {
        seed = UnityEngine.Random.Range(0, int.MaxValue);
        if (terrain == null)
        {
            Debug.LogError("Terrain object is not assigned.");
            return;
        }

        InitializeTextures();
        GeneratePerlinNoise();
        
        StartCoroutine(ColorTransition());
        StartCoroutine(SaveTextureAfterDelay("TerrainMap", 2f));
    }

    void InitializeTextures()
    {
        perlinTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
        heightMapTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
        pathTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);

        // Ensure terrain layers are set up
        if (heightMapLayer == null || perlinNoiseLayer == null || pathTextureLayer == null)
        {
            Debug.LogWarning("Terrain layers are not assigned. Please assign them in the inspector.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        terrainData.terrainLayers = new TerrainLayer[] { heightMapLayer, perlinNoiseLayer, pathTextureLayer };
    }

    #endregion

    #region Perlin Noise

    void GeneratePerlinNoise()
    {
        // Randomize parameters
        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        float heightScale = UnityEngine.Random.Range(heightScaleMin, heightScaleMax);
        float islandRadius = UnityEngine.Random.Range(islandRadiusMin, islandRadiusMax);
        float offsetX = UnityEngine.Random.Range(offsetXMin, offsetXMax);
        float offsetY = UnityEngine.Random.Range(offsetYMin, offsetYMax);

        // Ensure the terrain layers have correct tiling
        heightMapLayer.tileSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
        perlinNoiseLayer.tileSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);

        // Generate heightmap texture
        Texture2D heightmapTexture = new Texture2D(textureResolution, textureResolution);
        GenerateHeightmap(heightmapTexture, scale, heightScale, islandRadius, offsetX, offsetY);

        // Generate color map texture based on heightmap
        Texture2D colorMapTexture = new Texture2D(textureResolution, textureResolution);
        GenerateColorMap(heightmapTexture, colorMapTexture);

        // Apply textures to terrain
        ApplyHeightmapTexture(heightmapTexture);
        ApplyColorMapTexture(colorMapTexture);

        // Save textures
        SaveTexture(heightmapTexture, "HeightmapTexture");
        SaveTexture(colorMapTexture, "ColorMapTexture");
    }

    void GenerateHeightmap(Texture2D texture, float scale, float heightScale, float islandRadius, float offsetX, float offsetY)
    {
        for (int x = 0; x < textureResolution; x++)
        {
            for (int y = 0; y < textureResolution; y++)
            {
                float xCoord = (float)x / textureResolution * scale + offsetX;
                float yCoord = (float)y / textureResolution * scale + offsetY;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                // Calculate the distance from the center of the texture
                float centerX = textureResolution / 2f;
                float centerY = textureResolution / 2f;
                float distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
                float distanceNormalized = Mathf.InverseLerp(0, islandRadius, distance);

                // Blend the Perlin noise with a radial gradient to create the island effect
                float blendedSample = Mathf.Lerp(sample, 0f, distanceNormalized);

                Color heightColor = new Color(blendedSample, blendedSample, blendedSample); // Black, gray, white for heightmap
                texture.SetPixel(x, y, heightColor);
            }
        }

        texture.Apply();
    }

    void GenerateColorMap(Texture2D heightmap, Texture2D colorMap)
    {
        for (int x = 0; x < textureResolution; x++)
        {
            for (int y = 0; y < textureResolution; y++)
            {
                Color heightColor = heightmap.GetPixel(x, y);
                float heightValue = heightColor.r; // Use red channel for grayscale height

                Color mapColor = CalculateColorFromHeight(heightValue);
                colorMap.SetPixel(x, y, mapColor);
            }
        }

        colorMap.Apply();
    }

    Color CalculateColorFromHeight(float height)
    {
        if (height < threshold1)
        {
            return LowestLayer;
        }
        else if (height < threshold2)
        {
            return midLayer;
        }
        else
        {
            return TopLayer;
        }
    }

    void ApplyHeightmapTexture(Texture2D texture)
    {
        if (terrain != null)
        {
            TerrainData terrainData = terrain.terrainData;
            terrainData.heightmapResolution = textureResolution;
            terrainData.SetHeights(0, 0, ConvertToHeightmap(terrainData, texture));
        }
        else
        {
            Debug.LogError("Terrain component not found.");
        }
    }

    void ApplyColorMapTexture(Texture2D texture)
    {
        if (terrain != null && terrain.terrainData != null)
        {
            TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;

            if (terrainLayers.Length < 3)
            {
                Array.Resize(ref terrainLayers, 3);
                terrainLayers[2] = new TerrainLayer();
            }

            terrainLayers[2].diffuseTexture = texture;
            terrain.terrainData.terrainLayers = terrainLayers;
        }
        else
        {
            Debug.LogError("Terrain is not assigned or terrain data is missing.");
        }
    }

    float[,] ConvertToHeightmap(TerrainData terrainData, Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = texture.GetPixel(x, y);
                heights[x, y] = color.r; // Use red channel for height
            }
        }

        return heights;
    }

    void SaveTexture(Texture2D texture, string name)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.dataPath, "SavedTextures", name + ".png"), bytes);
        Debug.Log("Texture saved to " + Application.dataPath + "/SavedTextures/" + name + ".png");
    }

    #endregion

  

    #region Coroutines

    IEnumerator ColorTransition()
    {
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            Color currentLowestLayer = Color.Lerp(LowestLayer, targetLowestLayer, t);
            Color currentMidLayer = Color.Lerp(midLayer, targetMidLayer, t);
            Color currentTopLayer = Color.Lerp(TopLayer, targetTopLayer, t);

            // Update your terrain texture based on transition
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    IEnumerator SaveTextureAfterDelay(string textureName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SaveTexture(perlinTexture, textureName);
    }

    #endregion
}
