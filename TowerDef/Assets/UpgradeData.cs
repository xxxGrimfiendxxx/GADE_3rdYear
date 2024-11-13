using UnityEngine;

[CreateAssetMenu(fileName = "New UpgradeData", menuName = "Tower Defense/UpgradeData", order = 1)]
public class UpgradeData : ScriptableObject
{
    public int cost;                // The cost of the upgrade
    public float range;             // The range of the tower after upgrade
    public int maxHealth;           // The max health after upgrade
    public int damage;              // The damage dealt by the tower after upgrade
    public GameObject upgradedPrefab; // The new prefab for the tower (if it changes)
}
