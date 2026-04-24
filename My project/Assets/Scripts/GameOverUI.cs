using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    void Start()
    {
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore.ToString();
        }
    }
}