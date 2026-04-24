using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode placeKey = KeyCode.F;

    [Header("UI Prompt")]
    public Text promptText;

    [Header("Holding")]
    public Transform holdPoint;
    private GameObject heldItem = null;

    [Header("Placement")]
    private PlacementArea currentPlacementArea;

    private InteractableObject currentInteractable;
    private FoodItem currentFood;

    void Update()
    {
        FindInteractable();

        // Update UI prompt based on context
        if (heldItem != null && currentPlacementArea != null)
        {
            promptText.text = "Press F to place " + heldItem.GetComponent<FoodItem>().foodName;
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(placeKey))
            {
                PlaceHeldItem();
            }
            return;
        }
        else if (heldItem == null && currentPlacementArea != null && currentPlacementArea.HasVeggie)
        {
            promptText.text = "Press E to take vegetable";
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(interactKey))
            {
                TakeFromPlacement();
            }
            return;
        }
        else if (currentInteractable != null)
        {
            promptText.text = currentInteractable.interactionPrompt;
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(interactKey))
            {
                // If looking at chopping board and a veggie is placed, chop it
                if (currentInteractable.GetComponent<ChoppingBoard>() != null && currentPlacementArea != null && currentPlacementArea.HasVeggie)
                {
                    FoodItem placedFood = currentPlacementArea.RetrieveVeggie().GetComponent<FoodItem>();
                    if (placedFood != null)
                    {
                        placedFood.Chop();
                    }
                }
                else
                {
                    currentInteractable.Interact();
                    // If it's a food item and we're not holding anything, pick it up
                    if (currentFood != null && heldItem == null && currentFood.isGrabbable)
                    {
                        heldItem = currentFood.Pickup();
                        if (heldItem != null)
                        {
                            heldItem.transform.SetParent(holdPoint);
                            heldItem.transform.localPosition = Vector3.zero;
                            heldItem.transform.localRotation = Quaternion.identity;
                            heldItem.SetActive(true);
                        }
                    }
                }
            }
            return;
        }

        promptText.gameObject.SetActive(false);
    }

    void PlaceHeldItem()
    {
        if (heldItem == null || currentPlacementArea == null) return;
        currentPlacementArea.PlaceVeggie(heldItem);
        heldItem = null;
    }

    void TakeFromPlacement()
    {
        if (heldItem != null || currentPlacementArea == null || !currentPlacementArea.HasVeggie) return;
        heldItem = currentPlacementArea.RetrieveVeggie();
        heldItem.transform.SetParent(holdPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
        heldItem.SetActive(true);
    }

    void FindInteractable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        float closestDist = interactionRange + 1f;
        InteractableObject closest = null;
        FoodItem closestFood = null;
        PlacementArea closestPlace = null;

        foreach (var col in hitColliders)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist > closestDist) continue;

            InteractableObject io = col.GetComponent<InteractableObject>();
            PlacementArea pa = col.GetComponent<PlacementArea>();
            if (io != null)
            {
                closestDist = dist;
                closest = io;
                closestFood = col.GetComponent<FoodItem>();
            }
            if (pa != null)
            {
                closestDist = dist;
                closestPlace = pa;
            }
        }

        currentInteractable = closest;
        currentFood = closestFood;
        currentPlacementArea = closestPlace;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}