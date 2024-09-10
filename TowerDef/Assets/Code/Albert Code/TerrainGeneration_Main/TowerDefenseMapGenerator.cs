using UnityEngine;

public class TowerDefenseMapManager : MonoBehaviour
{
    public GameObject plane; // Assign the plane object in the inspector
    public Color pathColor = Color.green;
    public Color towerColor = Color.magenta;
    public int pathCount = 3;
    public float pathWidth = 3.0f;
    public float towerRadius = 40.0f;
    public float minTowerDistance = 10.0f;
    public float maxTowerDistance = 20.0f;
    public int towerSize = 5;

    private Texture2D mapTexture;
    private Renderer planeRenderer;
    private int width = 121;
    private int height = 121;

    private void Start()
    {
        if (plane == null)
        {
            Debug.LogError("Plane object is not assigned.");
            return;
        }

        InitializeTexture();
        GeneratePaths();
        MarkTowerPositions();
        ApplyTexture();
    }

    void InitializeTexture()
    {
        mapTexture = new Texture2D(width, height);
        planeRenderer = plane.GetComponent<Renderer>();

        if (planeRenderer == null)
        {
            Debug.LogError("Renderer not found on the plane object.");
            return;
        }

        // Create a new texture and set it to the plane's material
        mapTexture.filterMode = FilterMode.Bilinear;
        mapTexture.wrapMode = TextureWrapMode.Repeat;
        planeRenderer.material.mainTexture = mapTexture;
    }

    void GeneratePaths()
    {
        // Initialize paths starting points
        Vector2[] startPoints = new Vector2[]
        {
            new Vector2(width / 2, 0), // Bottom middle
            new Vector2(0, height / 2), // Top left
            new Vector2(width, height / 2) // Top right
        };

        Vector2 center = new Vector2(width / 2, height / 2);

        for (int i = 0; i < pathCount; i++)
        {
            Vector2 startPoint = startPoints[i];
            Vector2 currentPos = startPoint;
            Vector2 direction = (center - startPoint).normalized;

            while (Vector2.Distance(currentPos, center) > 1)
            {
                DrawCircle((int)currentPos.x, (int)currentPos.y, (int)pathWidth, pathColor);
                currentPos += direction * pathWidth;

                // Randomly change direction to create slithering effect
                direction = Quaternion.Euler(0, 0, Random.Range(-45, 45)) * direction;
            }

            // Ensure the path ends at the center
            DrawCircle((int)center.x, (int)center.y, (int)pathWidth, pathColor);
        }
    }

    void MarkTowerPositions()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapTexture.GetPixel(x, y) == pathColor)
                {
                    MarkTowersAround(x, y);
                }
            }
        }
    }

    void MarkTowersAround(int x, int y)
    {
        for (int i = 0; i < pathCount; i++)
        {
            float angle = i * Mathf.PI * 2 / pathCount;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 towerPosition = new Vector2(x, y) + direction * Random.Range(minTowerDistance, maxTowerDistance);

            if (towerPosition.x >= 0 && towerPosition.x < width && towerPosition.y >= 0 && towerPosition.y < height)
            {
                DrawSquare((int)towerPosition.x, (int)towerPosition.y, towerSize, towerColor);
            }
        }
    }

    void DrawCircle(int x, int y, int radius, Color color)
    {
        for (int dx = -radius; dx < radius; dx++)
        {
            for (int dy = -radius; dy < radius; dy++)
            {
                if (dx * dx + dy * dy <= radius * radius)
                {
                    SetPixel(x + dx, y + dy, color);
                }
            }
        }
    }

    void DrawSquare(int x, int y, int size, Color color)
    {
        for (int dx = -size / 2; dx < size / 2; dx++)
        {
            for (int dy = -size / 2; dy < size / 2; dy++)
            {
                SetPixel(x + dx, y + dy, color);
            }
        }
    }

    void SetPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            mapTexture.SetPixel(x, y, color);
        }
    }

    void ApplyTexture()
    {
        mapTexture.Apply();
    }
}
