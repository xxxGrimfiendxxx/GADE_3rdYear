using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavePushController : MonoBehaviour
{
    public TextMeshProUGUI waveText;       // Assign the TMP text component
    public float pushStrength = 10f;       // Adjust the pushing strength in the inspector
    public float pushDuration = 0.2f;      // Duration for which enemies are pushed
    public float pushRadius = 5f;          // Radius within which enemies are affected

    private string lastTextValue = "";     // Tracks the last text to detect changes
    private List<GameObject> pushedEnemies = new List<GameObject>(); // List of enemies being pushed

    void Start()
    {
        if (waveText != null)
            lastTextValue = waveText.text;
    }

    void Update()
    {
        // Check if the text has changed
        if (waveText != null && waveText.text != lastTextValue)
        {
            lastTextValue = waveText.text;  // Update the last text value
            StartCoroutine(PushEnemiesOutward());  // Start pushing enemies
        }
    }

    IEnumerator PushEnemiesOutward()
    {
        // Clear the list of previously pushed enemies
        pushedEnemies.Clear();

        // Find all enemies within the specified radius
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            // Check if the enemy is within the pushRadius from the cylinder's center
            if (Vector3.Distance(transform.position, enemy.transform.position) <= pushRadius)
            {
                pushedEnemies.Add(enemy);
            }
        }

        // Apply the push effect over the pushDuration
        float endTime = Time.time + pushDuration;
        while (Time.time < endTime)
        {
            foreach (GameObject enemy in pushedEnemies)
            {
                // Check if the enemy still exists
                if (enemy != null)
                {
                    // Calculate the push direction away from the cylinder's center
                    Vector3 pushDirection = (enemy.transform.position - transform.position).normalized;
                    enemy.transform.position += pushDirection * pushStrength * Time.deltaTime;
                }
            }

            yield return null;  // Wait until the next frame
        }

        // Clear the list after the push effect is over to avoid further interference
        pushedEnemies.Clear();
    }

    
    void OnValidate()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null && !collider.isTrigger)
        {
            Debug.LogWarning("The collider on this object is not set as a trigger. Setting it to trigger now.");
            collider.isTrigger = true;
        }
    }
}
