using System.Collections;
using UnityEngine;

public class TextureCollector : MonoBehaviour
{
    [Header("Objects with Textures")]
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;
    public GameObject object4;

    private Material material1;
    private Material material2;
    private Material material3;
    private Material material4;

    private Texture2D texture1;
    private Texture2D texture2;
    private Texture2D texture3;
    private Texture2D texture4;

    private void Start()
    {
        StartCoroutine(CollectTexturesAfterDelay(20f));
    }

    private IEnumerator CollectTexturesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Collect materials
        material1 = object1.GetComponent<Renderer>().material;
        material2 = object2.GetComponent<Renderer>().material;
        material3 = object3.GetComponent<Renderer>().material;
        material4 = object4.GetComponent<Renderer>().material;

        // Extract textures from materials
        texture1 = material1.mainTexture as Texture2D;
        texture2 = material2.mainTexture as Texture2D;
        texture3 = material3.mainTexture as Texture2D;
        texture4 = material4.mainTexture as Texture2D;

        // Do something with the textures
        ProcessTextures();
    }

    private void ProcessTextures()
    {
        if (texture1 != null) Debug.Log("Texture 1 collected");
        if (texture2 != null) Debug.Log("Texture 2 collected");
        if (texture3 != null) Debug.Log("Texture 3 collected");
        if (texture4 != null) Debug.Log("Texture 4 collected");

        // Further processing or usage of textures
        // For example, assigning them to other objects or combining them
    }
}
  