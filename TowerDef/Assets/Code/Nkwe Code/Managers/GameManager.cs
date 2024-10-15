using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Tower mainTower;
    public int currentWave = 1;
    public int totalWaves = 10;

    public float playerBaseHealth  ;
    public float maxPlayerBaseHealth;

    private void Start()
    {
        playerBaseHealth = mainTower.currentHealth;
        maxPlayerBaseHealth = mainTower.maxHealth;
        //UIManager.Instance.UpdateWaveInfo(currentWave, totalWaves); // Initialize wave info//p2
        UIManager.Instance.UpdatePlayerHealthUI(playerBaseHealth, maxPlayerBaseHealth); // Initialize base health UI
        
    }

    public void Update()
    {
        UpdateMT();
    }

    public void DamagePlayerBase(float damage)
    {
        playerBaseHealth -= damage;
        UIManager.Instance.UpdatePlayerHealthUI(playerBaseHealth, maxPlayerBaseHealth); // Update base health UI

        if (playerBaseHealth <= 0)
        {
            GameOver(false); // Game over if health reaches 0
        }
    }

    public void GameOver(bool won)
    {
        if (mainTower == null || mainTower.currentHealth == 0)
        {
            UIManager.Instance.ShowGameOver(won); // Show game over screen
        }
    }

    public void mainTowerUP()
    {
        if (mainTower != null && PlayerInfo.score >= 100)
        {
            GameOver(won:true);
        }

        if (mainTower == null && mainTower.currentHealth <= 0)
        {
            GameOver(won:false);
        }
    }

    public void UpdateMT()
    {
        mainTowerUP();
    }
    
    
}

