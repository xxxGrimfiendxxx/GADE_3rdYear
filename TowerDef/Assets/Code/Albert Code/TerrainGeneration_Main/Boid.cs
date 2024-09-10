using UnityEngine;

public class Boid : MonoBehaviour
{
    public enum BoidType { Type1, Type2, Type3 }
    public BoidType boidType;

    public float separationDistance = 1.0f;
    public float alignmentDistance = 2.0f;
    public float cohesionDistance = 2.0f;
    public float maxSpeed = 5.0f;
    public float maxForce = 0.1f;
    public float avoidRadius = 15.0f; // Radius to avoid around the center

    private Vector3 velocity;
    private Vector3 acceleration;

    public TerrainManager terrainManager; // Reference to TerrainManager
    public SpriteRenderer spriteRenderer; // Reference to SpriteRenderer for 2D painting
    public Color boidColor;

    public GameObject boundingBoxObject; // Reference to the bounding box GameObject
    private Bounds cubeBounds;

    private void Start()
    {
        velocity = Random.insideUnitSphere;

        if (boundingBoxObject != null)
        {
            cubeBounds = boundingBoxObject.GetComponent<Collider>().bounds;
        }
        else
        {
            Debug.LogWarning("BoundingBoxObject is not assigned.");
        }
    }

    private void Update()
    {
        ApplyBehavior();
        Move();
        ConstrainToBounds();
        PaintTerrain();
        PaintSprite(); // Paint the 2D sprite
    }

    private void Move()
    {
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;
        acceleration = Vector3.zero;
    }

    private void ApplyBehavior()
    {
        Vector3 sep = Vector3.zero;
        Vector3 ali = Vector3.zero;
        Vector3 coh = Vector3.zero;

        int total = 0;

        foreach (Boid boid in FindObjectsOfType<Boid>())
        {
            if (boid != this)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);

                // Apply separation regardless of boid type
                if (distance < separationDistance)
                {
                    Vector3 diff = transform.position - boid.transform.position;
                    sep += diff.normalized / distance; // Inverse weighting by distance
                }

                if (distance < alignmentDistance)
                {
                    ali += boid.velocity;
                }

                if (distance < cohesionDistance)
                {
                    coh += boid.transform.position;
                }

                total++;
            }
        }

        if (total > 0)
        {
            sep /= total;
            ali /= total;
            coh = (coh / total - transform.position).normalized;

            sep.Normalize();
            ali.Normalize();

            acceleration += sep * maxForce * 1.5f; // Increase separation force
            acceleration += ali * maxForce;
            acceleration += coh * maxForce * 0.5f; // Reduce cohesion slightly to encourage spreading
        }

        AvoidCentralRadius();
        SearchForUnpaintedSpaces();
    }

    private void AvoidCentralRadius()
    {
        // Calculate the center position of the terrain
        Vector3 center = Vector3.zero; // Assuming the center is at (0, 0, 0). Adjust as necessary.
        float distanceToCenter = Vector3.Distance(transform.position, center);

        if (distanceToCenter < avoidRadius)
        {
            Vector3 awayFromCenter = transform.position - center;
            acceleration += awayFromCenter.normalized * maxForce * 2.0f; // Increased force to avoid the center
        }
    }

    private void SearchForUnpaintedSpaces()
    {
        if (terrainManager != null)
        {
            Vector3 terrainPosition = transform.position;
            float terrainHeight = terrainManager.terrain.SampleHeight(terrainPosition);
            terrainPosition.y = terrainHeight;

            // Check the color of the terrain at the boid's current position
            Color pixelColor = terrainManager.GetColorAtPosition(terrainPosition);

            if (pixelColor == Color.blue) // Assuming blue is the color for unpainted areas
            {
                // If it's blue, encourage the boid to move towards this spot
                Vector3 directionToTarget = (terrainPosition - transform.position).normalized;
                acceleration += directionToTarget * maxForce;
            }
        }
    }

    private void ConstrainToBounds()
    {
        if (boundingBoxObject != null)
        {
            Vector3 pos = transform.position;
            if (!cubeBounds.Contains(pos))
            {
                Vector3 clampedPos = cubeBounds.ClosestPoint(pos);
                transform.position = clampedPos;
            }
        }
    }

    private void PaintTerrain()
    {
        if (terrainManager != null)
        {
            if (terrainManager.paintRadius > 0)
            {
                terrainManager.Paint(transform.position, boidColor);
            }
        }
    }

    private void PaintSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = boidColor;
        }
    }
}
