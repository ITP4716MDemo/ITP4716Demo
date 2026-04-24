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
    public PlacementArea placementArea;   // drag your counter here

    [Header("Pointer")]
    public Transform pointerTransform;

    [Header("Audio")]
    public AudioClip chopSound;          // assign in Inspector
    public AudioClip pickupSound;        // assign in Inspector
    private AudioSource audioSource;     // for playing sounds

    // ➡️ NEW: Reference to the fridge respawner
    [Header("Respawn")]
    public FridgeRespawner fridgeRespawner;   // drag the GameObject with FridgeRespawner here

    private InteractableObject currentInteractable;
    private FoodItem currentFood;
    private string currentHitName;
    private bool isLookingAtPlacementArea = false;

    void Start()
    {
        if (pointerTransform == null)
            pointerTransform = transform.Find("Pointer");
        if (pointerTransform == null)
            Debug.LogWarning("Pointer Transform not assigned – will fallback to camera raycast.");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        FindInteractableRaycast();

        // Update isLookingAtPlacementArea separately (could also be set in raycast)
        isLookingAtPlacementArea = (currentHitName == placementArea?.gameObject.name);

        // PLACEMENT prompt & action: must be holding item AND looking at the placement area
        if (heldItem != null && isLookingAtPlacementArea && placementArea != null && !placementArea.IsFull)
        {
            promptText.text = "Press F to place " + heldItem.GetComponent<FoodItem>().foodName;
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(placeKey))
            {
                PlaceHeldItem();
                return;
            }
        }
        // TAKING prompt: when empty-handed, looking at placement area that has a veggie
        else if (heldItem == null && isLookingAtPlacementArea && placementArea != null && placementArea.HasVeggie)
        {
            promptText.text = "Press E to take vegetable";
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(interactKey))
            {
                TakeFromPlacement();
                return;
            }
        }
        // OTHER interactable objects (including chopping board, items to pick up)
        else if (currentInteractable != null)
        {
            promptText.text = currentInteractable.interactionPrompt;
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(interactKey))
            {
                // Chopping board logic (must look at BoardRounded and have veggies placed)
                if (currentInteractable.GetComponent<ChoppingBoard>() != null &&
                    placementArea != null && placementArea.HasVeggie &&
                    currentHitName == "BoardRounded")
                {
                    FoodItem placedFood = placementArea.RetrieveFirstVeggie().GetComponent<FoodItem>();
                    if (placedFood != null)
                    {
                        if (chopSound != null && audioSource != null)
                            audioSource.PlayOneShot(chopSound);
                        placedFood.Chop();
                    }
                }
                else
                {
                    currentInteractable.Interact();
                    // Pickup from fridge or other objects
                    if (currentFood != null && heldItem == null && currentFood.isGrabbable)
                    {
                        heldItem = currentFood.Pickup();
                        if (heldItem != null)
                        {
                            if (pickupSound != null && audioSource != null)
                                audioSource.PlayOneShot(pickupSound);

                            // ➡️ NEW: Respawn a new vegetable in the fridge
                            if (fridgeRespawner != null)
                                fridgeRespawner.RespawnOneVegetable();

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
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    void FindInteractableRaycast()
    {
        Ray ray;
        if (pointerTransform != null)
        {
            ray = new Ray(pointerTransform.position, pointerTransform.forward);
            Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red, 2f);
        }
        else
        {
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.green, 2f);
        }

        int spawnAreaLayer = LayerMask.NameToLayer("SpawnArea");
        int layerMask = (spawnAreaLayer == -1) ? ~0 : ~(1 << spawnAreaLayer);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactionRange, layerMask))
        {
            currentHitName = hit.collider.gameObject.name;
            currentInteractable = hit.collider.GetComponent<InteractableObject>();
            currentFood = hit.collider.GetComponent<FoodItem>();
        }
        else
        {
            currentHitName = "";
            currentInteractable = null;
            currentFood = null;
        }
    }

    void PlaceHeldItem()
    {
        if (heldItem == null || placementArea == null) return;
        placementArea.PlaceVeggie(heldItem);
        heldItem = null;
    }

    void TakeFromPlacement()
    {
        if (heldItem != null || placementArea == null || !placementArea.HasVeggie) return;
        heldItem = placementArea.RetrieveFirstVeggie();
        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);
        heldItem.transform.SetParent(holdPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
        heldItem.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        if (pointerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(pointerTransform.position, pointerTransform.forward * interactionRange);
        }
    }
}