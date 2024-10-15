using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton instance for easy access
    private GameManager gameManager;

    [Header("Currency UI")]
    public TextMeshProUGUI currencyText; // Changed to TextMeshProUGUI to match TMP best practice
    public TextMeshProUGUI scoreText;

    [Header("Player Base Health UI")]
    public Image playerHealthBar;

    [Header("Wave Info UI")]
    public TextMeshProUGUI waveInfoText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverMessage;

    [Header("Tower Build UI")]
    public GameObject towerBuildPanel;
    public Button[] towerButtons; // Tower buttons to build towers

    [Header("Pause Menu")] 
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public bool isPaused;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one UIManager exists
        }

        // Initialize UI elements
        gameOverPanel?.SetActive(false); // Hide game over panel initially
        pausePanel.SetActive(false);
        isPaused = false;
        UpdateCurrencyUI(PlayerInfo.currency); // Set initial currency
        UpdateScoreUI(PlayerInfo.score); // Set initial score
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            TogglePause();
        }
        
    }

    // Updates the currency UI
    public void UpdateCurrencyUI(int currency)
    {
        if (currencyText != null)
        {
            currencyText.text = "Currency: " + currency.ToString();
        }
    }

    // Updates the player base health UI
    public void UpdatePlayerHealthUI(float currentHealth, float maxHealth)
    {
        if (playerHealthBar != null)
        {
            float healthPercentage = currentHealth / maxHealth;
            playerHealthBar.fillAmount = healthPercentage;
        }
    }

    // Updates the wave information UI//p2
    public void UpdateWaveInfo(int currentWave, int totalWaves)
    {
        if (waveInfoText != null)
        {
            waveInfoText.text = "Wave: " + currentWave + " / " + totalWaves;
        }
    }

    // Displays game over UI
    public void ShowGameOver(bool won)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverMessage.text = won ? "Victory!" : "Defeat!";
        }
    }

    // Tower Build UI can be used to enable/disable tower buttons based on available currency
    public void UpdateTowerButtons(int currentCurrency, int[] towerCosts)
    {
        for (int i = 0; i < towerButtons.Length; i++)
        {
            if (towerButtons[i] != null)
            {
                bool canAfford = currentCurrency >= towerCosts[i];
                towerButtons[i].interactable = canAfford;
                var buttonColors = towerButtons[i].colors;
                buttonColors.normalColor = canAfford ? Color.white : Color.gray;
                towerButtons[i].colors = buttonColors;
            }
        }
    }

    // Updates the player's score UI
    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    //  Reset the UI (useful for game restart)
    public void ResetUI()
    {
        UpdateCurrencyUI(PlayerInfo.currency);
        UpdateScoreUI(PlayerInfo.score);
        UpdatePlayerHealthUI(PlayerInfo.health,gameManager.mainTower.maxHealth); 
        gameOverPanel?.SetActive(false);// Hide game over panel
        pausePanel.SetActive(false);
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
        ResetUI();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Pauses the game
    public void PauseGame()
    {
        Time.timeScale = 0f;  // Freeze time
        isPaused = true;
        pausePanel.SetActive(true);
    }

    // Resumes the game
    public void ResumeGame()
    {
        Time.timeScale = 1f;  // Resume time
        isPaused = false;
        pausePanel.SetActive(false);
    }
}
