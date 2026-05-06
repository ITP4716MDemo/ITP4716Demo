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
        if (other.CompareTag("Pot"))
        {
            if (manager != null)
                manager.OnFoodEnteredPot(foodID);
            Destroy(gameObject);
        }
    }
}