using UnityEngine;
using System.Collections;

public class WormController : MonoBehaviour
{
    public Transform[] markers; // Path markers
    public Transform center; // Final center position
    public float moveSpeed = 1f; // Movement speed
    public float shapeChangeDuration = 1f; // Duration of shape change
    public float scaleFactor = 2f; // Factor by which the worm scales
    public Color wormColor; // Color of the worm

    private int currentTargetIndex = 0;
    private bool isChangingShape = false;

    private Vector3 boundingBoxMin;
    private Vector3 boundingBoxMax;

    private void Start()
    {
        // Set the worm's color
        GetComponent<Renderer>().material.color = wormColor;

        // Initialize bounding box size
        boundingBoxMin = new Vector3(-5, 0, -5); // Adjust these values according to your bounding box
        boundingBoxMax = new Vector3(5, 5, 5);

        // Start moving the worm
        StartCoroutine(MoveWorm());
    }

    private IEnumerator MoveWorm()
    {
        while (currentTargetIndex < markers.Length)
        {
            Vector3 targetPosition = markers[currentTargetIndex].position;

            // Move towards the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                if (!isChangingShape)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                    // Trigger shape change
                    if (Vector3.Distance(transform.position, targetPosition) < 1f)
                    {
                        isChangingShape = true;
                        StartCoroutine(ChangeShape());
                    }
                }
                yield return null;
            }

            // Move to the next marker
            currentTargetIndex++;
        }

        // Move to the center position after all markers are reached
        while (Vector3.Distance(transform.position, center.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, center.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator ChangeShape()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * scaleFactor; // Adjust scale as needed

        float elapsedTime = 0f;

        while (elapsedTime < shapeChangeDuration)
        {
            float t = elapsedTime / shapeChangeDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            // Ensure one side of the worm stays fixed to the bounding box
            AdjustPositionToBoundingBox();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
        isChangingShape = false;
    }

    private void AdjustPositionToBoundingBox()
    {
        Vector3 newPos = transform.position;
        Vector3 newScale = transform.localScale;

        // Calculate bounding constraints
        float halfWidth = newScale.x / 2;
        float halfHeight = newScale.y / 2;
        float halfDepth = newScale.z / 2;

        newPos.x = Mathf.Clamp(newPos.x, boundingBoxMin.x + halfWidth, boundingBoxMax.x - halfWidth);
        newPos.y = Mathf.Clamp(newPos.y, boundingBoxMin.y + halfHeight, boundingBoxMax.y - halfHeight);
        newPos.z = Mathf.Clamp(newPos.z, boundingBoxMin.z + halfDepth, boundingBoxMax.z - halfDepth);

        transform.position = newPos;
    }
}
