using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

public class PerlinOne : MonoBehaviour
{
    [Header("References")]
    public Material planeMaterial; // Assign if using a plane
    public Texture2D perlinTexture;

    [Header("Perlin Noise Settings")]
    public float scale = 20f;
    public float heightScale = 2f;
    public float borderWidth = 10f; // Width of the border area

    [Header("Texture Settings")]
    public int width = 121;  // Width of the texture
    public int height = 121; // Height of the texture
    public float islandRadius = 90f; // Radius of the island effect
    public float offsetX = 5f; // X offset for the noise
    public float offsetY = 5f; // Y offset for the noise

    [Header("Layer Colors")]
    public Color LowestLayer = new Color(0.0f, 0.75f, 0.5f);
    public Color midLayer = new Color(0.0f, 0.75f, 0.0f);
    public Color TopLayer = new Color(0.5f, 1.0f, 0.5f);

    [Header("Target Colors for Transition")]
    public Color targetLowestLayer = new Color(0.2f, 0.5f, 1.0f); // New color for transition
    public Color targetMidLayer = new Color(0.2f, 0.6f, 0.3f);     // New color for transition
    public Color targetTopLayer = new Color(0.7f, 0.8f, 0.2f);     // New color for transition

    [Header("Thresholds")]
    public float threshold1 = 0.1f; // Threshold for dark green
    public float threshold2 = 0.2f; // Threshold for mid green

    [Header("TerrainSettings")]
    public bool isCentralTerrain;
    private bool textureGenerated = false;
    // Mark if this is the central terrain

    private LandscapeManager landscapeManager;

    private void Start()
    {
        // Get reference to LandscapeManager
        landscapeManager = FindObjectOfType<LandscapeManager>();

        if (landscapeManager == null)
        {
            Debug.LogError("LandscapeManager not found in the scene.");
            return;
        }

        // Generate and apply the Perlin noise texture
        perlinTexture = GenerateIslandPerlinNoiseTexture(width, height);
        ApplyTextureToPlane(perlinTexture);

        // Notify LandscapeManager about the generated texture
        //landscapeManager.OnPerlinTextureGenerated(perlinTexture);

        // Start the color transition coroutine
        StartCoroutine(TransitionColors());

        // Optionally save the Perlin noise texture
        SavePerlinTexture(perlinTexture);
    }

    public void GeneratePerlinTexture()
    {
        // Implement the texture generation here
        perlinTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        // Populate perlinTexture with Perlin noise
        textureGenerated = true;
    }

    public bool IsTextureGenerated()
    {
        return textureGenerated;
    }

    public Texture2D GetGeneratedTexture()
    {
        return perlinTexture;
    }

    public Texture2D GenerateIslandPerlinNoiseTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color perlinColor = CalculateColor(x, y);
                texture.SetPixel(x, y, perlinColor);
            }
        }

        texture.Apply();
        return texture;
    }

    private Color CalculateColor(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        // Calculate the distance from the center of the texture
        float centerX = width / 2f;
        float centerY = height / 2f;
        float distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
        float distanceNormalized = Mathf.InverseLerp(0, islandRadius, distance);

        // Blend the Perlin noise with a radial gradient to create the island effect
        float blendedSample = Mathf.Lerp(sample, 0f, distanceNormalized);

        if (isCentralTerrain)
        {
            return CalculateMapColor(blendedSample);
        }
        else
        {
            // For surrounding terrains, match the central terrain's edge height
            if (IsBorderPixel(x, y))
            {
                blendedSample = 0.1f; // Adjust this to match the lowest layer of the central hill
            }
            return CalculateMapColor(blendedSample);
        }
    }

    private bool IsBorderPixel(int x, int y)
    {
        // Check if this pixel is on the border touching the central terrain
        return (x < borderWidth || x > width - borderWidth || y < borderWidth || y > height - borderWidth);
    }

    private Color CalculateMapColor(float brightness)
    {
        if (brightness < threshold1)
        {
            return LowestLayer;
        }
        else if (brightness < threshold2)
        {
            return midLayer;
        }
        else
        {
            return TopLayer;
        }
    }

    public void ApplyTextureToPlane(Texture2D texture)
    {
        if (planeMaterial != null)
        {
            planeMaterial.mainTexture = texture;
        }
        else
        {
            Debug.LogError("Plane material is not assigned.");
        }
    }

    public void SavePerlinTexture(Texture2D texture)
    {
        if (texture != null)
        {
            string folderPath = Path.Combine(Application.dataPath, "SavedTextures");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, "PerlinNoiseTexture.png");

            byte[] bytes = texture.EncodeToPNG();
            try
            {
                File.WriteAllBytes(filePath, bytes);
                Debug.Log("Texture saved to: " + filePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to save texture: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Perlin noise texture is null, cannot save.");
        }
    }

    public IEnumerator TransitionColors()
    {
        float duration = 10f; // Duration for the transition
        float elapsedTime = 0f;

        // Store initial colors
        Color startLowestLayer = LowestLayer;
        Color startMidLayer = midLayer;
        Color startTopLayer = TopLayer;

        while (elapsedTime < duration)
        {
            // Lerp colors over time
            LowestLayer = Color.Lerp(startLowestLayer, targetLowestLayer, elapsedTime / duration);
            midLayer = Color.Lerp(startMidLayer, targetMidLayer, elapsedTime / duration);
            TopLayer = Color.Lerp(startTopLayer, targetTopLayer, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            // Regenerate the texture with updated colors
            perlinTexture = GenerateIslandPerlinNoiseTexture(width, height);
            ApplyTextureToPlane(perlinTexture);

            yield return null; // Wait for the next frame
        }

        // Ensure the final colors are exactly set at the end
        LowestLayer = targetLowestLayer;
        midLayer = targetMidLayer;
        TopLayer = targetTopLayer;

        // Regenerate the texture one last time to apply the final colors
        perlinTexture = GenerateIslandPerlinNoiseTexture(width, height);
        ApplyTextureToPlane(perlinTexture);
    }
}
