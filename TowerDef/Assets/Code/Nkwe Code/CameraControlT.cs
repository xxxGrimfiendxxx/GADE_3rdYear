using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Speed of camera movement (panning)
    public float panBorderThickness = 10f; // Thickness of screen edge for panning

    public float zoomSpeed = 10f; // Speed of zooming in and out
    public float minZoom = 5f;    // Minimum zoom distance
    public float maxZoom = 20f;   // Maximum zoom distance

    public Vector2 panLimitX; // Limits for panning on the X-axis
    public Vector2 panLimitZ; // Limits for panning on the Z-axis

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    // Function to handle camera panning (moving)
    void HandleMovement()
    {
        Vector3 pos = transform.position;

        // Keyboard controls (WASD or arrow keys) or moving mouse to screen edge
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        // Clamp the camera's position to keep it within boundaries
        pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
        pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);

        // Set the camera's position
        transform.position = pos;
    }

    // Function to handle zooming in and out
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;

        pos.y -= scroll * zoomSpeed * 100f * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom); // Clamping the zoom limits

        transform.position = pos;
    }
}

