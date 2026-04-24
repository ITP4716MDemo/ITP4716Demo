using UnityEngine;

public class FoodItem : MonoBehaviour
{
    public string foodName = "Food";
    public bool isChopped = false;
    public bool isGrabbable = true;

    public void Chop()
    {
        if (isChopped) return;
        isChopped = true;
        ChoppingBoard board = FindObjectOfType<ChoppingBoard>();
        if (board != null)
            board.ChopFood(gameObject);
        else
            Destroy(gameObject);
    }

    public GameObject Pickup()
    {
        if (!isGrabbable) return null;
        gameObject.SetActive(false);
        return gameObject;
    }
}