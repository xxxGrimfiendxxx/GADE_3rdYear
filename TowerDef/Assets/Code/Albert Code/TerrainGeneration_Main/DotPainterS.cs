using System.Collections;
using System.IO;
using UnityEngine;

public class DotPainterS : MonoBehaviour
{
    public Texture2D persistentTexture;
    public Color centralColor = Color.green;
    public float radius = 15.0f;
    public float paintDelay = 2.0f; // Delay before painting the central area

    private Renderer planeRenderer;

    private void Start()
    {
        // Get the Renderer component of the plane
        planeRenderer = GetComponent<Renderer>();

        if (planeRenderer == null)
        {
            Debug.LogError("No Renderer component found on this plane.");
            return;
        }

        // Initialize the persistent texture
        InitializeTexture();

        // Start the painting process with a delay
        StartCoroutine(PaintAfterDelay());
    }

    private void InitializeTexture()
    {
        // Create a new texture if it’s not assigned
        if (persistentTexture == null)
        {
            int width = 121; // Example width; adjust as needed
            int height = 121; // Example height; adjust as needed
            persistentTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            ClearTexture(persistentTexture, Color.blue); 
        }
    }

    private IEnumerator PaintAfterDelay()
    {
        yield return new WaitForSeconds(paintDelay);
        PaintCentralArea();
        ApplyTextureToMaterial();
        SaveTextureToFile();
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

    public void PaintCentralArea()
    {
        if (persistentTexture == null)
        {
            Debug.LogError("PersistentTexture is not assigned.");
            return;
        }

        Vector3 planeSize = new Vector3(persistentTexture.width, 0, persistentTexture.height);
        Vector3 center = new Vector3(planeSize.x / 2, 0, planeSize.z / 2);

        for (int x = 0; x < persistentTexture.width; x++)
        {
            for (int z = 0; z < persistentTexture.height; z++)
            {
                float tx = Mathf.InverseLerp(0, persistentTexture.width, x);
                float tz = Mathf.InverseLerp(0, persistentTexture.height, z);

                Vector3 pixelWorldPos = new Vector3(
                    Mathf.Lerp(0, planeSize.x, tx),
                    0,
                    Mathf.Lerp(0, planeSize.z, tz)
                );

                if (Vector3.Distance(pixelWorldPos, center) < radius)
                {
                    persistentTexture.SetPixel(x, z, centralColor);
                }
            }
        }

        persistentTexture.Apply();
    }

    private void ApplyTextureToMaterial()
    {
        if (planeRenderer != null)
        {
            planeRenderer.material.mainTexture = persistentTexture;
        }
        else
        {
            Debug.LogError("Renderer component is missing.");
        }
    }

    private void SaveTextureToFile()
    {
        byte[] textureBytes = persistentTexture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, "SavedTextures");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filePath = Path.Combine(path, "PaintedTextureSmall.png");
        File.WriteAllBytes(filePath, textureBytes);

        Debug.Log($"Texture saved to {filePath}");
    }
}
