using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    [Header("Texture Paths")]
    private string savedTexturesPath;
    private readonly string[] textureOrder = { "PerlinNoiseTexture", "BoidTexture", "LargeDotTexture", "SmallDotTexture" };

    [Header("Assigned Textures")]
    public Texture2D perlinTexture;
    public Texture2D replaceDotTexture;
    public Texture2D boidTexture;
    public Texture2D dotTexture;

    public GameObject finalTexturePlane;
    public GameObject boidTexturePlane;
    public GameObject perlinTexturePlane;
    public GameObject smallDotTexturePlane;
    public GameObject largeDotTexturePlane;

    // Store the texture and save it
    public void StoreBoidTexture(Texture2D texture)
    {
        // Logic to store the boid texture
        SaveTextureToFile(texture, boidTexturePath);
    }

    // Example method to save texture to file
    private void SaveTextureToFile(Texture2D texture, string path)
    {
        byte[] textureBytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, textureBytes);
    }
    public string boidTexturePath;
    public string perlinTexturePath;
    public string dotTextureSmallPath;
    public string dotTextureLargePath;

    // Assuming color properties
    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;

    private int mapSize = 121; // Assuming all textures are 121x121

    private void Start()
    {
        savedTexturesPath = Path.Combine(Application.dataPath, "SavedTextures");

        if (Directory.Exists(savedTexturesPath))
        {
            StartCoroutine(DelayedTextureUpdate());
        }
        else
        {
            Debug.LogError("SavedTextures directory does not exist.");
        }
    }

    private IEnumerator DelayedTextureUpdate()
    {
        yield return new WaitForSeconds(20f); // Wait for 20 seconds before processing

        ProcessTextures();
        Texture2D finalTexture = CombineTextures();
        SaveTextureAsPNG(finalTexture, "Assets/FinalTexture.png");

        // Assign the textures to the planes
        AssignTexturesToPlanes();
    }

    private void ProcessTextures()
    {
        foreach (string textureName in textureOrder)
        {
            string filePath = Path.Combine(savedTexturesPath, textureName + ".png");
            if (File.Exists(filePath))
            {
                Texture2D texture = LoadTexture(filePath);
                if (texture != null)
                {
                    ProcessTexture(texture, textureName);
                }
            }
            else
            {
                Debug.LogWarning($"{textureName} not found in SavedTextures.");
            }
        }
    }

    private Texture2D LoadTexture(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // Load the image data into the texture
        return texture;
    }

    private void ProcessTexture(Texture2D texture, string textureName)
    {
        Color[] pixels = texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] == Color.blue)
            {
                pixels[i] = Color.clear; // Ignore blue pixels by making them transparent
            }
            else
            {
                pixels[i] = ProcessPixelColor(pixels[i]); // Process other colors
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Set textures based on their type
        switch (textureName)
        {
            case "PerlinNoiseTexture":
                perlinTexture = texture;
                break;
            case "BoidTexture":
                boidTexture = texture;
                break;
            case "SmallDotTexture":
                dotTexture = texture;
                break;
            case "LargeDotTexture":
                replaceDotTexture = texture;
                break;
            default:
                Debug.Log($"Unhandled texture type: {textureName}");
                break;
        }

        // Optionally save the processed texture again
        SaveTexture(texture, textureName + "_Processed");
    }

    private Color ProcessPixelColor(Color color)
    {
        // Apply color-specific processing here
        if (color == Color.red)
        {
            // Do something with red
        }
        else if (color == Color.green)
        {
            // Do something with green
        }
        else if (color == Color.yellow)
        {
            // Do something with yellow
        }
        else if (color == Color.magenta)
        {
            // Do something with magenta
        }

        return color; // Return the processed color
    }

    public void SaveTexture(Texture2D texture, string fileName)
    {
        if (texture != null)
        {
            byte[] bytes = texture.EncodeToPNG();
            string path = Path.Combine(savedTexturesPath, fileName + ".png");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, bytes);
            Debug.Log($"{fileName} saved at {path}");
        }
        else
        {
            Debug.LogError($"{fileName} is null, cannot save.");
        }
    }

    private Texture2D CombineTextures()
    {
        Texture2D finalTexture = new Texture2D(mapSize, mapSize);

        // Iterate through each pixel
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                // Start with the Perlin noise texture
                Color finalColor = perlinTexture.GetPixel(x, y);

                // Apply the boid texture
                Color boidColor = boidTexture.GetPixel(x, y);
                if (IsHigherLayer(finalColor, boidColor))
                {
                    finalColor = boidColor;
                }

                // Apply the dot texture (overrides to layer 2)
                Color dotColor = dotTexture.GetPixel(x, y);
                if (dotColor == Color.yellow)
                {
                    finalColor = Color.yellow;
                }

                // Apply the replacement dot (overrides layer 0 to 1)
                Color replaceDotColor = replaceDotTexture.GetPixel(x, y);
                if (replaceDotColor == Color.green && finalColor == Color.red)
                {
                    finalColor = Color.green;
                }

                finalTexture.SetPixel(x, y, finalColor);
            }
        }

        finalTexture.Apply();
        return finalTexture;
    }

    bool IsHigherLayer(Color currentColor, Color newColor)
    {
        // Define the priority of layers
        int currentLayer = GetLayerFromColor(currentColor);
        int newLayer = GetLayerFromColor(newColor);

        return newLayer > currentLayer;
    }

    int GetLayerFromColor(Color color)
    {
        // Returns the layer based on the color
        if (color == Color.red) return 0;
        if (color == Color.green) return 1;
        if (color == Color.yellow) return 2;
        if (color == Color.magenta) return 3;
        return -1; // Undefined or error case
    }

    public void SaveTextureAsPNG(Texture2D texture, string filePath)
    {
        if (texture != null)
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            Debug.Log($"Texture saved at {filePath}");
        }
        else
        {
            Debug.LogError("Texture is null, cannot save.");
        }
    }

    private void AssignTexturesToPlanes()
    {
        // Assign textures to the planes
        if (finalTexturePlane != null)
        {
            finalTexturePlane.GetComponent<Renderer>().material.mainTexture = CombineTextures();
        }

        if (boidTexturePlane != null && boidTexture != null)
        {
            boidTexturePlane.GetComponent<Renderer>().material.mainTexture = boidTexture;
        }

        if (perlinTexturePlane != null && perlinTexture != null)
        {
            perlinTexturePlane.GetComponent<Renderer>().material.mainTexture = perlinTexture;
        }

        if (smallDotTexturePlane != null && dotTexture != null)
        {
            smallDotTexturePlane.GetComponent<Renderer>().material.mainTexture = dotTexture;
        }

        if (largeDotTexturePlane != null && replaceDotTexture != null)
        {
            largeDotTexturePlane.GetComponent<Renderer>().material.mainTexture = replaceDotTexture;
        }
    }
}
