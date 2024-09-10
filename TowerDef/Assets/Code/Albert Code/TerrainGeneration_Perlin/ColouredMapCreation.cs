using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColouredMapCreation : MonoBehaviour
{
    public PerlinNoiseTexture perlinNoiseSource; // Reference to the previous PerlinNoiseTexture script
    public Color darkGreen = new Color(0.0f, 0.5f, 0.0f);
    public Color midGreen = new Color(0.0f, 0.75f, 0.0f);
    public Color lightGreen = new Color(0.5f, 1.0f, 0.5f);
    public float threshold1 = 0.33f; // Threshold for dark green
    public float threshold2 = 0.66f; // Threshold for mid green

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateMapTexture(perlinNoiseSource.GenerateTexture());
    }

    Texture2D GenerateMapTexture(Texture2D perlinTexture)
    {
        int width = perlinTexture.width;
        int height = perlinTexture.height;

        Texture2D mapTexture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color perlinColor = perlinTexture.GetPixel(x, y);
                float brightness = perlinColor.grayscale; // Get the brightness of the Perlin noise

                Color mapColor = CalculateMapColor(brightness);
                mapTexture.SetPixel(x, y, mapColor);
            }
        }

        mapTexture.Apply();
        return mapTexture;
    }

    Color CalculateMapColor(float brightness)
    {
        if (brightness < threshold1)
        {
            return darkGreen;
        }
        else if (brightness < threshold2)
        {
            return midGreen;
        }
        else
        {
            return lightGreen;
        }
    }

}
