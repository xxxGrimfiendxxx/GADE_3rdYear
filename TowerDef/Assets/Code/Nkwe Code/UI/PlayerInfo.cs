using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
    public static int currency; // Player's current currency
    public int startCurrency = 500; // Initial currency at the start of the game
    public int passiveIncomeAmount = 5; // Amount of currency gained passively
    public float passiveIncomeInterval = 5f; // Time interval for passive income (in seconds)

    public int killIncomeAmount = 20; // Amount of currency gained per kill

    public static int health; // Player's base health
    public int startHealth = 100; // Initial health of the player's base

    public static int score; // Player's score
    public int pointsPerEnemy = 10; // Points earned for each defeated enemy

    public UIManager uiManager; // Reference to the UI Manager to update currency, health, and game-over screen

    private void Start()
    {
        currency = startCurrency;
        score = 0;

        // Update UI at the start
        if (uiManager != null)
        {
            uiManager.UpdateCurrencyUI(currency);
            uiManager.UpdateScoreUI(score);
        }

        // Start passive income coroutine
        StartCoroutine(PassiveIncomeCoroutine());
    }

    // Coroutine to generate passive income at regular intervals
    private IEnumerator PassiveIncomeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveIncomeInterval); // Wait for the specified interval
            GainCurrencyPassive(); // Add passive income
        }
    }

    // Function to gain passive income
    public void GainCurrencyPassive()
    {
        currency += passiveIncomeAmount;

        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateCurrencyUI(currency);
        }
    }

    // Function to spend currency
    public bool SpendCurrency(int amount)
    {
        if (currency >= amount)
        {
            currency -= amount;

            // Update UI
            if (uiManager != null)
            {
                uiManager.UpdateCurrencyUI(currency);
            }

            return true;
        }
        else
        {
            Debug.Log("Not enough currency!");
            return false;
        }
    }

    // Function to gain points (e.g., when enemies are defeated)
    public void GainScore(int enemyValue)
    {
        score += enemyValue * pointsPerEnemy;

        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateScoreUI(score);
        }
    }

    // Function to gain currency from enemy kills
    public void GainCurrencyKill(int enemyValue)
    {
        currency += killIncomeAmount * enemyValue;

        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateCurrencyUI(currency);
        }
    }
}


