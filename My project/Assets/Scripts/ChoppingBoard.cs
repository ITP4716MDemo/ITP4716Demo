using UnityEngine;

public class ChoppingBoard : MonoBehaviour
{
    public GameObject choppedFoodPrefab;
    public Transform spawnPoint;

    public void ChopFood(GameObject food)
    {
        Destroy(food);
        if (choppedFoodPrefab != null && spawnPoint != null)
        {
            Instantiate(choppedFoodPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}