using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Reference to the UI Slider component
    public Transform target; // The object the health bar will follow
    public Vector3 offset; // Offset to place the health bar above the object

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

    /*private void Update()//2d 
    {
        if (target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
        }
    }*/
    private void LateUpdate()
    {
        if (target != null)
        {
            // Set the health bar's position to match the target's world position + offset
            transform.position = target.position + offset;

            // Make the health bar always face the camera (billboarding effect)
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); // Correct for the health bar facing away
        }
    }
}

