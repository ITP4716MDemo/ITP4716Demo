using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PointsDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Text uiText;           // For legacy Text
    [SerializeField] private TextMeshProUGUI tmpText; // For TextMeshPro

    [Header("Display Settings")]
    [SerializeField] private string prefix = "Points: ";

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found! Make sure GameManager exists in the scene.");
            enabled = false;
            return;
        }

        UpdateDisplay();
    }

    void Update()
    {
        // Update every frame (or use events for better performance)
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (GameManager.Instance == null) return;

        int points = GameManager.Instance.TotalPoints;
        string displayText = prefix + points.ToString();

        if (uiText != null) uiText.text = displayText;
        if (tmpText != null) tmpText.text = displayText;
    }
}