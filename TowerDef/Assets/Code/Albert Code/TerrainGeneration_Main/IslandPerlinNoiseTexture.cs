using UnityEngine;
using System.IO;


public class MixedIslandPerlinNoiseTexture : MonoBehaviour
{
    public Material planeMaterial; // Assign if using a plane
    public Texture2D perlinTexture;
    public float scale = 20f;
    public float heightScale = 2f;
    public float borderWidth = 10f; // Width of the border area

    public int width = 121;  // Width of the texture
    public int height = 121; // Height of the texture
    public float islandRadius = 90f; // Radius of the island effect
    public float offsetX = 5f; // X offset for the noise
    public float offsetY = 5f; // Y offset for the noise

    public Color LowestLayer = new Color(0.0f, 0.75f, 0.5f);
    public Color midLayer = new Color(0.0f, 0.75f, 0.0f);
    public Color TopLayer = new Color(0.5f, 1.0f, 0.5f);
    public float threshold1 = 0.1f; // Threshold for dark green
    public float threshold2 = 0.2f; // Threshold for mid green

    private void Start()
    {
        if (planeMaterial != null)
        {
            // Generate and apply the Perlin noise texture
            perlinTexture = GenerateIslandPerlinNoiseTexture(width, height);
            ApplyTextureToPlane(perlinTexture);

            // Optionally save the Perlin noise texture
            SavePerlinTexture(perlinTexture);
        }
        else
        {
            Debug.LogError("Plane material is not assigned!");
        }
    }

    private Texture2D GenerateIslandPerlinNoiseTexture(int width, int height)
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

        return CalculateMapColor(blendedSample);
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

    private void ApplyTextureToPlane(Texture2D texture)
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

    private void SavePerlinTexture(Texture2D texture)
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
}
