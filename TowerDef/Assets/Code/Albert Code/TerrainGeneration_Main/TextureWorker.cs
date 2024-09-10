using System.Collections;
using UnityEngine;


public class TextureWorker : MonoBehaviour
{
    public TextureManager textureManager; // Reference to TextureManager

    public Terrain terrain;
    public Texture2D heightmapTexture;

    private int mapSize = 121; // Assuming the heightmap texture size is 121x121

    private void Start()
    {
        ApplyHeightmap();
        StartCoroutine(TransitionColorsOverTime(50f)); // Start color transition over 50 seconds
    }

    private void ApplyHeightmap()
    {
        if (terrain == null || heightmapTexture == null)
        {
            Debug.LogError("Terrain or heightmap texture not set.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        float[,] heights = new float[mapSize, mapSize];

        // Convert the heightmap texture into height values
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Color heightColor = heightmapTexture.GetPixel(x, y);
                heights[x, y] = heightColor.grayscale; // Use the grayscale value as the height
            }
        }

        terrainData.heightmapResolution = mapSize;
        terrainData.size = new Vector3(mapSize, 10, mapSize); // Adjust size; 10 is the height range
        terrainData.SetHeights(0, 0, heights);
    }

    private IEnumerator TransitionColorsOverTime(float duration)
    {
        float elapsedTime = 0f;
        

        // Capture the initial colors from the TextureManager
        Color startColor1 = textureManager.color1;
        Color startColor2 = textureManager.color2;
        Color startColor3 = textureManager.color3;
        Color startColor4 = textureManager.color4;

        // Define the target colors for the transition
        Color endColor1 = Color.white; // Example end color
        Color endColor2 = Color.white;
        Color endColor3 = Color.white;
        Color endColor4 = Color.white;

        // Gradually transition colors over the specified duration
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Calculate the transition ratio

            // Lerp the colors from start to end over time
            textureManager.color1 = Color.Lerp(startColor1, endColor1, t);
            textureManager.color2 = Color.Lerp(startColor2, endColor2, t);
            textureManager.color3 = Color.Lerp(startColor3, endColor3, t);
            textureManager.color4 = Color.Lerp(startColor4, endColor4, t);

            elapsedTime += Time.deltaTime; // Update elapsed time
            yield return null; // Wait for the next frame
        }

        // Set the final colors once the transition is complete
        textureManager.color1 = endColor1;
        textureManager.color2 = endColor2;
        textureManager.color3 = endColor3;
        textureManager.color4 = endColor4;
    }
}
