using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class FridgeShopUI : MonoBehaviour
{
    public GameObject shopPanel;               // the UI panel
    public Transform buttonContainer;          // parent for dynamic buttons
    public GameObject buttonPrefab;            // UI button prefab (with TextMeshPro)
    public TextMeshProUGUI totalPointsText;    // optional: display total points

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found! Make sure it exists in the scene.");
            return;
        }

        // ⚠️ Removed: shopPanel.SetActive(false); – shop is now visible on scene load
        BuildShopUI();
    }

    void Update()
    {
        // Update points display if assigned
        if (totalPointsText != null && GameManager.Instance != null)
            totalPointsText.text = "Total Points: " + GameManager.Instance.TotalPoints;
    }

    void BuildShopUI()
    {
        // Clear existing buttons
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (var food in GameManager.Instance.unlockableFoods)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                string status = food.isUnlocked ? "UNLOCKED" : $"Cost: {food.unlockCost}";
                btnText.text = $"{food.foodID}\n{status}";
            }

            Button button = btnObj.GetComponent<Button>();
            if (!food.isUnlocked)
            {
                string foodID = food.foodID; // capture for lambda
                button.onClick.AddListener(() => TryBuyFood(foodID));
            }
            else
            {
                button.interactable = false; // already unlocked
            }
        }
    }

    void TryBuyFood(string foodID)
    {
        if (GameManager.Instance.TryUnlockFood(foodID))
        {
            Debug.Log($"Purchased {foodID}!");
            BuildShopUI(); // refresh
        }
        else
        {
            Debug.Log("Not enough points!");
        }
    }

    // Optional: you can keep ToggleShop() if you ever need it,
    // but it's not required for the shop to appear.
    public void ToggleShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
        if (shopPanel.activeSelf)
        {
            BuildShopUI();
            if (totalPointsText != null)
                totalPointsText.text = "Total Points: " + GameManager.Instance.TotalPoints;
        }
    }
}