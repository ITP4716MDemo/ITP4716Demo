using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class CookingMinigameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Game Settings")]
    public float gameTime = 60f;
    public Transform handPoint;
    public GameObject[] foodPrefabs;
    public string potTag = "Pot";
    public string nextSceneName = "GameOver";

    [Header("Score Values")]
    private Dictionary<string, int> foodScores = new Dictionary<string, int>
    {
        { "Squash01", 5 },
        { "Cabbage01", 5 },
        { "Carrot01", 4 },
        { "Corgette01", 3 },
        { "Potato02", 3 },
        { "Potato01", 2 },
        { "SweetPotato01", 2 },
        { "Garlic01", 1 },
        { "Onion01", 1 },
        { "Turnip01", 1 },
        { "Mush01", 1 },
        { "Mush02", 2 }
    };

    [System.Serializable]
    public class UnlockableFood
    {
        public string foodID;
        public int unlockCost;
        public bool isUnlocked;
    }
    public UnlockableFood[] unlockableFoods;

    private float timeRemaining;
    private int currentScore = 0;
    private bool isGameActive = true;
    private GameObject currentHeldFood = null;

    private const string TOTAL_POINTS_KEY = "CookingTotalPoints";
    private const string UNLOCK_KEY_PREFIX = "Unlocked_";

    void Start()
    {
        LoadUnlockStates();
        timeRemaining = gameTime;
        UpdateUI();

        PlayerLockPosition lockPos = FindObjectOfType<PlayerLockPosition>();
        if (lockPos != null) lockPos.LockPlayer(true);

        SpawnRandomFood();
    }

    void Update()
    {
        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isGameActive = false;
            UpdateUI();
            EndGame();
            return;
        }
        UpdateUI();

        if (currentHeldFood != null && Input.GetMouseButtonDown(0))
        {
            ThrowFood();
        }
    }

    private void UpdateUI()
    {
        UpdateScoreUI();
        UpdateTimerUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore.ToString();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = "Time: " + seconds.ToString();
        }
    }

    private void SpawnRandomFood()
    {
        if (currentHeldFood != null)
            Destroy(currentHeldFood);

        List<string> unlockedIDs = unlockableFoods
            .Where(f => f.isUnlocked)
            .Select(f => f.foodID)
            .ToList();

        if (unlockedIDs.Count == 0)
        {
            Debug.LogError("No unlocked foods! Unlock at least one in the fridge.");
            return;
        }

        string selectedID = unlockedIDs[Random.Range(0, unlockedIDs.Count)];
        GameObject prefab = foodPrefabs.FirstOrDefault(p => p.name == selectedID);
        if (prefab == null)
        {
            Debug.LogError($"No prefab found for food ID '{selectedID}'");
            return;
        }

        currentHeldFood = Instantiate(prefab, handPoint.position, handPoint.rotation, handPoint);
        Rigidbody rb = currentHeldFood.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        ThrowableFood tf = currentHeldFood.GetComponent<ThrowableFood>();
        if (tf == null) tf = currentHeldFood.AddComponent<ThrowableFood>();
        tf.Initialize(this, selectedID);
    }

    private void ThrowFood()
    {
        if (currentHeldFood == null) return;

        currentHeldFood.transform.SetParent(null);
        Rigidbody rb = currentHeldFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            Vector3 throwDir = Camera.main.transform.forward;
            rb.AddForce(throwDir * 15f, ForceMode.Impulse);
        }

        currentHeldFood = null;
        Invoke(nameof(SpawnRandomFood), 0.5f);
    }

    public void OnFoodEnteredPot(string foodID)
    {
        if (!isGameActive) return;

        if (!foodScores.ContainsKey(foodID))
        {
            Debug.LogWarning($"Unknown food: {foodID}");
            return;
        }

        int points = foodScores[foodID];
        currentScore += points;

        int total = PlayerPrefs.GetInt(TOTAL_POINTS_KEY, 0);
        total += points;
        PlayerPrefs.SetInt(TOTAL_POINTS_KEY, total);
        PlayerPrefs.Save();

        UpdateUI();
        Debug.Log($"Threw {foodID} into pot! +{points} points. Total: {currentScore}");
    }

    private void EndGame()
    {
        Debug.Log($"Game over! Final score: {currentScore}");

        PlayerLockPosition lockPos = FindObjectOfType<PlayerLockPosition>();
        if (lockPos != null) lockPos.LockPlayer(false);

        PlayerPrefs.SetInt("FinalScore", currentScore);
        PlayerPrefs.Save();

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogError("nextSceneName is not set!");
    }

    private void LoadUnlockStates()
    {
        foreach (var food in unlockableFoods)
        {
            string key = UNLOCK_KEY_PREFIX + food.foodID;
            if (PlayerPrefs.HasKey(key))
                food.isUnlocked = PlayerPrefs.GetInt(key) == 1;
            // else keep inspector default
        }
    }

    public bool TryUnlockFood(string foodID)
    {
        var food = unlockableFoods.FirstOrDefault(f => f.foodID == foodID);
        if (food == null || food.isUnlocked) return false;

        int totalPoints = PlayerPrefs.GetInt(TOTAL_POINTS_KEY, 0);
        if (totalPoints >= food.unlockCost)
        {
            PlayerPrefs.SetInt(TOTAL_POINTS_KEY, totalPoints - food.unlockCost);
            food.isUnlocked = true;
            string key = UNLOCK_KEY_PREFIX + foodID;
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
            Debug.Log($"Unlocked {foodID} for {food.unlockCost} points!");
            return true;
        }
        return false;
    }
}