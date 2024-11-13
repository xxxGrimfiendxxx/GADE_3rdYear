using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseTexture : MonoBehaviour
{
    public int width = 121;  // Width of the texture
    public int height = 121; // Height of the texture
    public float scale = 20f; // Scale of the noise
    public float offsetX = 100f; // X offset for the noise
    public float offsetY = 100f; // Y offset for the noise
    public float islandRadius = 50f; // Radius of the island effect

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    public Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    Color CalculateColor(int x, int y)
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

        return new Color(blendedSample, blendedSample, blendedSample);
    }
}
