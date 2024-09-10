using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LandscapeManager : MonoBehaviour
{
    public Terrain[] terrains;
    public TerrainLayer[] terrainLayers;
    public RenderTexture renderTexture;
    public float paintRadius = 1.0f;
    public Color initialColor = Color.green;
    public Color targetColor = Color.red;
    public float transitionDuration = 10.0f;
    public List<PerlinOne> perlinScripts;
    public PathManager pathManager;
    public GameObject perlinPlane;
    public GameObject pathPlane;
    public int textureWidth = 121;
    public int textureHeight = 121;

    private Texture2D[] persistentTextures;
    private int textureResolution = 121;
    private Dictionary<string, Terrain> terrainDict;

    private void Start()
    {
        if (terrains == null || terrains.Length == 0 || perlinScripts == null || perlinScripts.Count == 0)
        {
            Debug.LogError("No terrains or Perlin scripts assigned!");
            return;
        }

        // Initialize persistent textures for each terrain
        persistentTextures = new Texture2D[terrains.Length];
        terrainDict = new Dictionary<string, Terrain>();

        for (int i = 0; i < terrains.Length; i++)
        {
            if (terrains[i] == null) continue;

            terrainDict.Add(terrains[i].name, terrains[i]);
            persistentTextures[i] = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
            ClearTexture(persistentTextures[i], initialColor);
            ApplyTextureToTerrain(terrains[i], persistentTextures[i]);
        }

        // Initialize Perlin noise texture
        PerlinOne perlinOneScript = perlinPlane.GetComponent<PerlinOne>();
        if (perlinOneScript == null)
        {
            Debug.LogError("PerlinOne script not found on Perlin plane object.");
            return;
        }
        perlinOneScript.width = textureWidth;
        perlinOneScript.height = textureHeight;
        perlinOneScript.GeneratePerlinTexture(); // Generate texture

        // Initialize PathManager
        PathManager pathManagerScript = pathPlane.GetComponent<PathManager>();
        if (pathManagerScript == null)
        {
            Debug.LogError("PathManager script not found on Path plane object.");
            return;
        }
        //pathManagerScript.width = textureWidth;
        //pathManagerScript.height = textureHeight;

        // Start generating terrain and pathways
        StartCoroutine(GenerateLandscape());

        // Start the color transition
        StartCoroutine(ColorTransition());

        // Save textures after delay
        StartCoroutine(SaveTextureAfterDelay("CentralTerrain", 20f));
    }

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

    private void ApplyHeightMaps()
    {
        foreach (var script in perlinScripts)
        {
            foreach (var terrain in terrains)
            {
                Texture2D perlinTexture = script.GetGeneratedTexture();
                ApplyTextureToTerrain(terrain, perlinTexture);
            }
        }
    }

    private void ApplyPerlinTextures()
    {
        foreach (var terrain in terrains)
        {
            foreach (var script in perlinScripts)
            {
                Texture2D perlinTexture = script.GetGeneratedTexture();
                if (perlinTexture != null)
                {
                    ApplyTextureToTerrain(terrain, perlinTexture);
                }
            }
        }
    }

    private IEnumerator GenerateLandscape()
    {
        // Start generating Perlin noise texture
        PerlinOne perlinOneScript = perlinPlane.GetComponent<PerlinOne>();
        perlinOneScript.GeneratePerlinTexture();
        yield return new WaitUntil(() => perlinOneScript.IsTextureGenerated());

        // Apply the generated Perlin texture
        Texture2D perlinTexture = perlinOneScript.GetGeneratedTexture();
        PathManager pathManagerScript = pathPlane.GetComponent<PathManager>();
        //pathManagerScript.ApplyPerlinTexture(perlinTexture);

        // Start generating paths
        //pathManagerScript.GeneratePathways();
        //yield return new WaitUntil(() => pathManagerScript.IsPathwaysGenerated());

        // Notify or update any other components if necessary
        Debug.Log("Landscape generation complete.");
    }

    private IEnumerator ColorTransition()
    {
        float elapsedTime = 0;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            for (int i = 0; i < persistentTextures.Length; i++)
            {
                Texture2D texture = persistentTextures[i];
                Color[] pixels = texture.GetPixels();

                for (int j = 0; j < pixels.Length; j++)
                {
                    pixels[j] = Color.Lerp(initialColor, targetColor, t);
                }

                texture.SetPixels(pixels);
                texture.Apply();
                UpdateTerrainTexture(terrains[i], texture);
            }

            yield return null;
        }
    }

    private void UpdateTerrainTexture(Terrain terrain, Texture2D texture)
    {
        TerrainData terrainData = terrain.terrainData;
        TerrainLayer[] layers = terrainData.terrainLayers;

        if (layers.Length > 0)
        {
            TerrainLayer layer = layers[0];
            layer.diffuseTexture = texture;
            layer.tileSize = new Vector2(terrainData.size.x, terrainData.size.z);
            terrainData.terrainLayers = layers;
        }
    }

    private void ApplyTextureToTerrain(Terrain terrain, Texture2D texture)
    {
        if (terrain.materialTemplate != null)
        {
            terrain.materialTemplate.mainTexture = texture;
        }
        else
        {
            Debug.LogError("Terrain material is not assigned.");
        }
    }

    private IEnumerator SaveTextureAfterDelay(string fileName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SaveTexture(fileName, persistentTextures[0]);
    }

    public void SaveTexture(string fileName, Texture2D texture)
    {
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
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
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save texture: " + ex.Message);
        }
    }
}
