using UnityEngine;
using TMPro; // For displaying currency if you use TextMeshPro

public class UpgradeManager : MonoBehaviour
{
    // Reference to the TMP currency text UI
    public TMP_Text currencyText;

    // Starting currency for the player
    public int currentCurrency = 1000;

    // The button that triggers the upgrade
    public GameObject upgradeButton;

    // Optional: Reference to the UI showing upgrade info (e.g., the cost of upgrade)
    public TMP_Text upgradeCostText;

    // Prefabs for base, level 2, and level 3 towers
    public GameObject[] baseTowers; // 3 base towers
    public GameObject[] level2Towers; // 3 level 2 towers
    public GameObject[] level3Towers; // 3 level 3 towers

    private Tower selectedTower;

    // Method to show the upgrade button when a tower is clicked
    public void ShowUpgradeButton(Tower tower)
    {
        if (tower == null || tower.currentUpgradeLevel >= tower.upgrades.Length - 1)
        {
            upgradeButton.SetActive(false);  // Hide the button if no upgrade is available
            return;
        }

        selectedTower = tower;
        upgradeButton.SetActive(true);  // Show upgrade button

        // Update the upgrade button with the correct cost
        int upgradeCost = tower.upgrades[tower.currentUpgradeLevel].cost;
        if (upgradeCostText != null)
        {
            upgradeCostText.text = "Upgrade Cost: " + upgradeCost.ToString();
        }

        // Set the button functionality to call UpgradeTower
        upgradeButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();  // Remove previous listeners
        upgradeButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => UpgradeTower(tower));
    }

    // Method to upgrade the tower
    public void UpgradeTower(Tower tower)
    {
        if (tower == null || tower.currentUpgradeLevel >= tower.upgrades.Length - 1) return;

        int upgradeCost = tower.upgrades[tower.currentUpgradeLevel].cost;

        // Check if the player has enough currency
        if (currentCurrency >= upgradeCost)
        {
            // Spend the currency
            currentCurrency -= upgradeCost;
            UpdateCurrencyUI(); // Update the UI with new currency value

            // Apply the upgrade stats
            tower.currentUpgradeLevel++;
            ApplyUpgrade(tower);

            // Optionally: Handle the tower prefab change if there is an upgraded version
            UpgradeData upgrade = tower.upgrades[tower.currentUpgradeLevel];
            if (upgrade.upgradedPrefab != null)
            {
                // Destroy the old tower
                Destroy(tower.gameObject);

                // Instantiate the upgraded prefab (base -> level 2 -> level 3)
                GameObject upgradedPrefab = GetUpgradedPrefab(tower.currentUpgradeLevel);
                if (upgradedPrefab != null)
                {
                    Instantiate(upgradedPrefab, tower.transform.position, tower.transform.rotation, tower.transform.parent);
                }
            }

            // Optionally update the upgrade button or UI elements
            upgradeButton.SetActive(false); // Hide the button after upgrade
        }
        else
        {
            Debug.Log("Not enough currency to upgrade!");
        }
    }

    // Method to get the correct prefab based on the tower's upgrade level
    private GameObject GetUpgradedPrefab(int level)
    {
        switch (level)
        {
            case 0:
                return baseTowers[0]; // Return base tower prefab
            case 1:
                return level2Towers[0]; // Return level 2 tower prefab
            case 2:
                return level3Towers[0]; // Return level 3 tower prefab
            default:
                return null; // If no valid level is found, return null
        }
    }

    // Method to update the currency UI text
    private void UpdateCurrencyUI()
    {
        currencyText.text = "Currency: " + currentCurrency.ToString();
    }

    // Apply upgrade to the tower (e.g., update damage, health, etc.)
    private void ApplyUpgrade(Tower tower)
    {
        UpgradeData upgrade = tower.upgrades[tower.currentUpgradeLevel];

        tower.range = upgrade.range;
        tower.maxHealth = upgrade.maxHealth;
        tower.towerDmg = upgrade.damage;
        tower.currentHealth = tower.maxHealth;

        // Update health bar UI
        tower.healthBar.SetMaxHealth(tower.maxHealth);
        tower.healthBar.SetHealth(tower.currentHealth);
    }

    // Method to close the upgrade UI (called when upgrade is completed or canceled)
    public void CloseUpgradeUI()
    {
        upgradeButton.SetActive(false); // Hide upgrade button
    }
}
