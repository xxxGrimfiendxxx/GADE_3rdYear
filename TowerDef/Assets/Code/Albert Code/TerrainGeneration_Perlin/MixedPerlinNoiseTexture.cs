using UnityEngine;
using System.IO;

public class MixedPerlinNoiseTexture : MonoBehaviour
{
    public Material planeMaterial; // Assign if using a plane
    public Texture2D perlinTexture;
    public float scale = 20f;
    public float heightScale = 2f;
    public float borderWidth = 10f; // Width of the border area

    private void Start()
    {
        if (planeMaterial != null)
        {
            // Using plane
            perlinTexture = GeneratePerlinNoiseTexture((int)GetPlaneWidth(), (int)GetPlaneHeight());
            ApplyTextureToPlane(perlinTexture);

            // Optionally save the Perlin noise texture
            SavePerlinTexture(perlinTexture);
        }
        else
        {
            Debug.LogError("Plane material is not assigned!");
        }
    }

    private Texture2D GeneratePerlinNoiseTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        float offsetX = Random.value * 100;
        float offsetY = Random.value * 100;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = offsetX + x / (float)width * scale;
                float yCoord = offsetY + y / (float)height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                texture.SetPixel(x, y, new Color(sample, sample, sample));
            }
        }

        texture.Apply();
        return texture;
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
            // Define the folder path and ensure it exists
            string folderPath = Path.Combine(Application.dataPath, "SavedTextures");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the file path
            string filePath = Path.Combine(folderPath, "PerlinNoiseTexture.png");

            // Convert texture to PNG and save to file
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


    private float GetPlaneWidth()
    {
        // Assuming the plane's width is 121 units
        return 121f;
    }

    private float GetPlaneHeight()
    {
        // Assuming the plane's height is 121 units
        return 121f;
    }
}
