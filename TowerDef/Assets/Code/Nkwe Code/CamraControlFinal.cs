using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamraControlFinal : MonoBehaviour
{
    private bool inputMov = true;
    public float panSpeed = 30f;
    public float panBorderThickness = 20f;
    
    public float zoomSpeed = 10f; // Speed of zooming in and out
    public float minZoom = 5f;    // Minimum zoom distance
    public float maxZoom = 200f;

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
        HandleZoom();
    }

    private void CameraMovement()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputMov = !inputMov;
        }
        if (!inputMov)
        {
            return;
        }
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height- panBorderThickness)
        {
            transform.Translate(Vector3.forward * panSpeed* Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
        {
            transform.Translate(Vector3.back * panSpeed* Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            transform.Translate(Vector3.right * panSpeed* Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.y <= panBorderThickness)
        {
            transform.Translate(Vector3.left * panSpeed* Time.deltaTime, Space.World);
        }
    }
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;

        pos.y -= scroll * zoomSpeed * 1000f * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom); // Clamping the zoom limits

        transform.position = pos;
    }
}
