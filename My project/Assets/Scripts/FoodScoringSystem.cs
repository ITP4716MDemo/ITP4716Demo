using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FoodScoringSystem : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public string nextSceneName = "GameOver";

    private Dictionary<string, int> foodScores;
    private int currentTotalScore = 0;
    private float timeRemaining = 60f;
    private bool timerRunning = true;

    private string lastChoppedFood = "";
    private int consecutiveSameCount = 0;

    void Awake()
    {
        InitializeFoodScores();
        UpdateScoreUI();
    }

    void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                UpdateTimerUI();
                SaveFinalScoreAndLoadNextScene();
            }
        }
    }

    void InitializeFoodScores()
    {
        foodScores = new Dictionary<string, int>
        {
            { "SM_Food_Squash01", 5 },
            { "SM_Food_Cabbage01", 5 },
            { "SM_Food_Carrot01", 3 },
            { "SM_Food_Corgette01", 3 },
            { "SM_Food_Potato02", 2 },
            { "SM_Food_Potato01", 2 },
            { "SM_SweetPotato01", 2 },
            { "SM_Sweet_Garlic01", 1 },
            { "SM_Food_Onion01", 1 },
            { "SM_Food_Turnip01", 1 },
            { "SM_Food_Mush01", 1 },
            { "SM_Food_Mush02", 1 }
        };
    }

    public void ChopFood(string foodName)
    {
        if (!foodScores.ContainsKey(foodName))
        {
            Debug.LogError("Unknown food type: " + foodName);
            return;
        }

        int basePoints = foodScores[foodName];
        int totalGained = basePoints;

        // Bonus for chopping the same food twice in a row
        if (foodName == lastChoppedFood)
        {
            totalGained += basePoints;  // bonus = base points
            consecutiveSameCount++;
            Debug.Log($"COMBO! Same food twice in a row! +{basePoints} bonus");
        }
        else
        {
            consecutiveSameCount = 1;
        }

        lastChoppedFood = foodName;
        currentTotalScore += totalGained;
        UpdateScoreUI();

        Debug.Log($"Chopped {foodName} → +{totalGained} points. Total: {currentTotalScore}");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentTotalScore.ToString();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = "Time: " + seconds.ToString();
        }
    }

    private void SaveFinalScoreAndLoadNextScene()
    {
        PlayerPrefs.SetInt("FinalScore", currentTotalScore);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nextSceneName);
    }

    public void ResetScore()
    {
        currentTotalScore = 0;
        lastChoppedFood = "";
        consecutiveSameCount = 0;
        UpdateScoreUI();
    }
}