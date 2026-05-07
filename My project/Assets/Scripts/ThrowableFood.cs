using UnityEngine;

public class ThrowableFood : MonoBehaviour
{
    private CookingMinigameManager manager;
    private string foodID;

    public void Initialize(CookingMinigameManager mgr, string id)
    {
        manager = mgr;
        foodID = id;
    }

    public string GetFoodID()
    {
        return foodID;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ThrowableFood] Hit: {other.name}, tag: {other.tag}");
        
        if (other.CompareTag("Pot") || other.CompareTag("PotSmall"))
        {
            Debug.Log($"[ThrowableFood] Pot detected. Manager is {(manager == null ? "NULL" : "assigned")}");
            if (manager != null)
            {
                Debug.Log($"[ThrowableFood] Calling manager.OnFoodEnteredPot with foodID={foodID}, pot={other.gameObject.name}");
                manager.OnFoodEnteredPot(foodID, other.gameObject);
            }
            else
            {
                Debug.LogError("[ThrowableFood] Manager reference is NULL! Cannot add points.");
            }
            Destroy(gameObject);
        }
    }
}