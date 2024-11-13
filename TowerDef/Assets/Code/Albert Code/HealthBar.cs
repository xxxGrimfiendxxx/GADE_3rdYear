using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Sprite[] healthBarSprites;  // Array to hold sprites for different tower levels (1, 2, 3)
    public Image healthBarRenderer;  // The SpriteRenderer for the health bar
    public float maxHealth = 100f;
    public float currentHealth;
    public float healthPercentage;

    public Transform target;  // Target tower for positioning the health bar
    public Vector3 offset = new Vector3(0, 2f, 0);  // Offset for the health bar position above the tower

    private void Start()
    {
        currentHealth = maxHealth;  // Initialize current health
        UpdateHealthBar();  // Update the health bar initially
    }

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
        currentHealth = max;
        UpdateHealthBar();
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        healthPercentage = currentHealth / maxHealth;
        UpdateHealthBar();
    }

    public void SetHealthBarSprite(int level)
    {
        // Change the health bar sprite based on the tower's level
        if (level >= 1 && level <= healthBarSprites.Length)
        {
            healthBarRenderer.sprite = healthBarSprites[level - 1];  // Set the sprite for the correct level
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarRenderer == null) return;  // Null check

        // Calculate health percentage and adjust the health bar visibility
        Vector3 scale = healthBarRenderer.transform.localScale;
        scale.x = healthPercentage;  // Adjust width from 0 to 1 (0% to 100%)
        healthBarRenderer.transform.localScale = scale;

        // Optionally, change color based on health
        Color color = Color.Lerp(Color.red, Color.green, healthPercentage);
        healthBarRenderer.color = color;
    }
}
