using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrefabPointTracker : MonoBehaviour
{

    [System.Serializable]
    public class PrefabData
    {
        public GameObject prefab; // Prefab reference
        public int pointValue;    // Points associated with the prefab
    }

    [Header("Prefab Data")]
    public List<PrefabData> prefabs = new List<PrefabData>(); // List of prefabs and their points

    [Header("UI Reference")]
    public TMP_Text scoreText; // TextMeshPro text box reference

    private void Start()
    {
        UpdateScore(); // Calculate score at the start
    }
    private void Update()
    {
        UpdateScore(); // Calculate score for the rest of the game
    }

    // This function counts the number of instances of each prefab in the scene and tallies the points
    public void UpdateScore()
    {
        int totalPoints = 0;

        foreach (var prefabData in prefabs)
        {
            if (prefabData.prefab != null)
            {
                // Find all instances of this prefab in the scene
                GameObject[] prefabInstances = GameObject.FindGameObjectsWithTag(prefabData.prefab.tag);
                int prefabCount = prefabInstances.Length;

                // Add the points for each instance of this prefab
                totalPoints += prefabCount * prefabData.pointValue;
            }
        }

        // Update the text box with the total points
        if (scoreText != null)
        {
            scoreText.text = "" + totalPoints;
        }
    }

    // Method to manually trigger score calculation from the Inspector
    [ContextMenu("Recalculate Score")]
    public void RecalculateScore()
    {
        UpdateScore();
    }
}
