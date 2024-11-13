using System.Collections;
using System.IO;
using UnityEngine;

public class TextureUpdater : MonoBehaviour
{
    public string folderPath = "Assets/SavedTextures"; // Path to the folder containing the textures
    public float updateInterval = 6f; // Time in seconds between each update check

    private void Start()
    {
        StartCoroutine(UpdateTexturesRoutine());
    }

    private IEnumerator UpdateTexturesRoutine()
    {
        while (true)
        {
            UpdateTextures();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateTextures()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"Folder not found: {folderPath}");
            return;
        }

        // Reload the textures from the folder
        string[] files = Directory.GetFiles(folderPath, "*.png");

        foreach (string file in files)
        {
            byte[] fileData = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            // Optional: Apply additional logic with the textures if needed
            Debug.Log($"Updated texture: {Path.GetFileName(file)} with size {texture.width}x{texture.height}");
        }
    }
}
