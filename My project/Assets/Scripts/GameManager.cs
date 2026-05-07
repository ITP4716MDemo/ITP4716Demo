using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Points")]
    private const string TOTAL_POINTS_KEY = "CookingTotalPoints";
    public int TotalPoints 
    { 
        get => PlayerPrefs.GetInt(TOTAL_POINTS_KEY, 0);
        set { PlayerPrefs.SetInt(TOTAL_POINTS_KEY, value); PlayerPrefs.Save(); }
    }

    [System.Serializable]
    public class UnlockableFood
    {
        public string foodID;
        public int unlockCost;
        public bool isUnlocked;
    }
    public UnlockableFood[] unlockableFoods;

    private const string UNLOCK_KEY_PREFIX = "Unlocked_";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllUnlockStates();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllUnlockStates()
    {
        foreach (var food in unlockableFoods)
        {
            string key = UNLOCK_KEY_PREFIX + food.foodID;
            if (PlayerPrefs.HasKey(key))
                food.isUnlocked = PlayerPrefs.GetInt(key) == 1;
            else
            {
                // Default unlocked: Mush01, Turnip01, Onion01
                bool defaultUnlock = (food.foodID == "Mush01" || food.foodID == "Turnip01" || food.foodID == "Onion01");
                food.isUnlocked = defaultUnlock;
                if (defaultUnlock)
                    PlayerPrefs.SetInt(key, 1);
            }
        }
        PlayerPrefs.Save();
    }

    public void AddPoints(int amount)
    {
        TotalPoints += amount;
    }

    public bool TryUnlockFood(string foodID)
    {
        var food = unlockableFoods.FirstOrDefault(f => f.foodID == foodID);
        if (food == null || food.isUnlocked) return false;

        if (TotalPoints >= food.unlockCost)
        {
            TotalPoints -= food.unlockCost;
            food.isUnlocked = true;
            PlayerPrefs.SetInt(UNLOCK_KEY_PREFIX + foodID, 1);
            PlayerPrefs.Save();
            Debug.Log($"Unlocked {foodID} for {food.unlockCost} points!");
            return true;
        }
        return false;
    }

    public bool IsFoodUnlocked(string foodID)
    {
        var food = unlockableFoods.FirstOrDefault(f => f.foodID == foodID);
        return food != null && food.isUnlocked;
    }
}