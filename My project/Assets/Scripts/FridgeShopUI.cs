using UnityEngine;
using TMPro;
using System.Linq;

public class FridgeShopUI : MonoBehaviour
{
    public GameObject shopPanel;               // the UI panel
    public Transform buttonContainer;          // parent for dynamic buttons
    public GameObject buttonPrefab;            // UI button prefab (with TextMeshPro)
    private CookingMinigameManager gameManager; // reference (or find it)

    void Start()
    {
        gameManager = FindObjectOfType<CookingMinigameManager>();
        if (gameManager == null)
        {
            Debug.LogError("CookingMinigameManager not found in scene!");
            return;
        }
        shopPanel.SetActive(false);
        BuildShopUI();
    }

    void BuildShopUI()
    {
        // Clear existing
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (var food in gameManager.unlockableFoods)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                string status = food.isUnlocked ? "UNLOCKED" : $"Cost: {food.unlockCost}";
                btnText.text = $"{food.foodID}\n{status}";
            }

            UnityEngine.UI.Button button = btnObj.GetComponent<UnityEngine.UI.Button>();
            if (!food.isUnlocked)
            {
                button.onClick.AddListener(() => TryBuyFood(food.foodID));
            }
            else
            {
                button.interactable = false; // already unlocked
            }
        }
    }

    void TryBuyFood(string foodID)
    {
        if (gameManager.TryUnlockFood(foodID))
        {
            Debug.Log($"Purchased {foodID}!");
            BuildShopUI(); // refresh
            // Also update total points display if you have one
        }
        else
        {
            Debug.Log("Not enough points!");
        }
    }

    public void ToggleShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
        if (shopPanel.activeSelf) BuildShopUI();
    }
}