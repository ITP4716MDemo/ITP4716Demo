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
    public string potTag = "Pot";               // General pot tag
    public string doublePointsPotTag = "PotSmall"; // Tag that gives double points
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
        if (GameManager.Instance == null)
            Debug.LogError("GameManager instance not found! Please add GameManager to your first scene with DontDestroyOnLoad.");

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

        if (GameManager.Instance == null || GameManager.Instance.unlockableFoods == null || GameManager.Instance.unlockableFoods.Length == 0)
        {
            Debug.LogError("GameManager missing or has no unlockableFoods array!");
            return;
        }

        var unlocked = GameManager.Instance.unlockableFoods.Where(f => f.isUnlocked).ToList();
        if (unlocked.Count == 0)
        {
            Debug.LogWarning("No unlocked foods. Unlocking default set (Mush01, Turnip01, Onion01).");
            foreach (var f in GameManager.Instance.unlockableFoods)
            {
                if (f.foodID == "Mush01" || f.foodID == "Turnip01" || f.foodID == "Onion01")
                    f.isUnlocked = true;
            }
            unlocked = GameManager.Instance.unlockableFoods.Where(f => f.isUnlocked).ToList();
        }

        List<GameObject> availablePrefabs = new List<GameObject>();
        foreach (var food in unlocked)
        {
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
        string selectedID = selectedPrefab.name.Replace("SM_Food_", "");

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

    // NEW: Accepts the pot GameObject and checks its tag for double points (with debug logs)
    public void OnFoodEnteredPot(string foodID, GameObject pot)
    {
        Debug.Log($"[CookingManager] OnFoodEnteredPot called: foodID={foodID}, pot={(pot != null ? pot.name : "NULL")}, pot tag={(pot != null ? pot.tag : "NULL")}");

        if (!isGameActive) return;

        if (!foodScores.ContainsKey(foodID))
        {
            Debug.LogWarning($"Unknown food: {foodID}");
            return;
        }

        int points = foodScores[foodID];
        bool isDouble = false;

        if (pot != null && pot.CompareTag(doublePointsPotTag))
        {
            points *= 2;
            isDouble = true;
            Debug.Log($"✅ DOUBLE POINTS! +{points} for {foodID} in pot tagged '{doublePointsPotTag}'");
        }
        else
        {
            Debug.Log($"❌ Normal points: +{points} for {foodID}. Pot tag is '{(pot != null ? pot.tag : "NULL")}', expected '{doublePointsPotTag}' for double.");
        }

        currentScore += points;

        if (GameManager.Instance != null)
            GameManager.Instance.AddPoints(points);

        UpdateUI();
        Debug.Log($"Threw {foodID} into pot! +{points} points. Total this minigame: {currentScore}");
    }

    // Old overload for compatibility (if any script still calls with one argument)
    public void OnFoodEnteredPot(string foodID)
    {
        Debug.LogWarning($"[CookingManager] Old overload called for {foodID} – no pot reference, cannot double points.");
        OnFoodEnteredPot(foodID, null);
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
}