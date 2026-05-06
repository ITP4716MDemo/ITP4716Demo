using UnityEngine;

public class FridgeInteract : MonoBehaviour
{
    public FridgeShopUI shopUI;

    void Start()
    {
        if (shopUI == null)
            shopUI = FindObjectOfType<FridgeShopUI>();
    }

    public void Interact()
    {
        if (shopUI != null)
            shopUI.ToggleShop();
        else
            Debug.LogWarning("FridgeShopUI not found in scene!");
    }
}