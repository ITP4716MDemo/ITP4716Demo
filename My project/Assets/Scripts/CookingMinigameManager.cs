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
        { "Garlic01", 2 },
        { "Onion01", 1 },
        { "Turnip01", 1 },
        { "Mush01", 1 },
        { "Mush02", 2 }
    };

    private float timeRemaining;
    private int currentScore = 0;
    private bool isGameActive = true;
    private GameObject currentHeldFood = null;

    void Start()
    {
        // Ensure GameManager exists
        if (GameManager.Instance == null)
            Debug.LogError("GameManager instance not found! Please add GameManager to your first scene with DontDestroyOnLoad.");

        timeRemaining = gameTime;
        UpdateUI();

        // Lock player movement
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
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore.ToString();
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

        // Get unlocked foods from GameManager
        if (GameManager.Instance == null || GameManager.Instance.unlockableFoods == null || GameManager.Instance.unlockableFoods.Length == 0)
        {
            Debug.LogError("GameManager missing or has no unlockableFoods array!");
            return;
        }

        var unlocked = GameManager.Instance.unlockableFoods.Where(f => f.isUnlocked).ToList();
        if (unlocked.Count == 0)
        {
            Debug.LogWarning("No unlocked foods. Unlocking default set (Mush01, Turnip01, Onion01).");
            // Force default unlock (should already be true from GameManager, but safety)
            foreach (var f in GameManager.Instance.unlockableFoods)
            {
                if (f.foodID == "Mush01" || f.foodID == "Turnip01" || f.foodID == "Onion01")
                    f.isUnlocked = true;
            }
            unlocked = GameManager.Instance.unlockableFoods.Where(f => f.isUnlocked).ToList();
        }

        // Build list of available prefabs
        List<GameObject> availablePrefabs = new List<GameObject>();
        foreach (var food in unlocked)
        {
            // Try exact name match or with "SM_Food_" prefix
            GameObject prefab = foodPrefabs.FirstOrDefault(p => p != null && (p.name == food.foodID || p.name == "SM_Food_" + food.foodID));
            if (prefab != null)
                availablePrefabs.Add(prefab);
            else
                Debug.LogWarning($"Missing prefab for unlocked food '{food.foodID}'. Add it to foodPrefabs array.");
        }

        if (availablePrefabs.Count == 0)
        {
            Debug.LogError("No valid unlocked foods with prefabs! Check your foodPrefabs array.");
            return;
        }

        GameObject selectedPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
        string selectedID = selectedPrefab.name.Replace("SM_Food_", ""); // get clean ID

        currentHeldFood = Instantiate(selectedPrefab, handPoint.position, handPoint.rotation, handPoint);
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

        // Add points persistently via GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.AddPoints(points);

        UpdateUI();
        Debug.Log($"Threw {foodID} into pot! +{points} points. Total this minigame: {currentScore}");
    }

    private void EndGame()
    {
        Debug.Log($"Game over! Final score: {currentScore}");

        PlayerLockPosition lockPos = FindObjectOfType<PlayerLockPosition>();
        if (lockPos != null) lockPos.LockPlayer(false);

        // Optionally save final score for result screen
        PlayerPrefs.SetInt("FinalScore", currentScore);
        PlayerPrefs.Save();

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogError("nextSceneName is not set!");
    }
}