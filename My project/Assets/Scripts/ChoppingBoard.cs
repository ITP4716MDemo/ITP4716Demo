using UnityEngine;

public class ChoppingBoard : MonoBehaviour
{
    public GameObject choppedFoodPrefab;
    public Transform spawnPoint;

    private FoodScoringSystem scoringSystem;

    void Start()
    {
        scoringSystem = FindObjectOfType<FoodScoringSystem>();
        if (scoringSystem == null)
        {
            Debug.LogError("FoodScoringSystem not found in scene!");
        }
    }

    public void ChopFood(GameObject food)
    {
        string foodName = food.name;
        // Remove "(Clone)" if the food is instantiated
        if (foodName.Contains("(Clone)"))
        {
            foodName = foodName.Replace("(Clone)", "").Trim();
        }

        // Award points before destroying
        if (scoringSystem != null)
        {
            scoringSystem.ChopFood(foodName);
        }
        else
        {
            Debug.LogWarning("No scoring system – points not added");
        }

        Destroy(food);

        if (choppedFoodPrefab != null && spawnPoint != null)
        {
            Instantiate(choppedFoodPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}