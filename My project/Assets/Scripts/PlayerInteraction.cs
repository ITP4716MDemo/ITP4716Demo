using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Prompt")]
    public Text promptText;

    [Header("Pointer")]
    public Transform pointerTransform;

    private DoorLevelSelector currentDoor;
    private FridgeInteract currentFridge;   // <-- ADDED: fridge interaction
    private string currentHitName;

    void Start()
    {
        if (pointerTransform == null)
            pointerTransform = transform.Find("Pointer");
        if (pointerTransform == null)
            Debug.LogWarning("Pointer Transform not assigned – will fallback to camera raycast.");
    }

    void Update()
    {
        FindInteractableRaycast();

        // --- PRIORITY 1: Looking at a fridge ---
        if (currentFridge != null)
        {
            promptText.text = "Press E to open fridge shop";
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(interactKey))
            {
                currentFridge.Interact();
            }
            return;
        }

        // --- PRIORITY 2: Looking at a door ---
        if (currentDoor != null)
        {
            promptText.text = "Press E to select level";
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(interactKey))
            {
                currentDoor.ShowSelectionMenu();
            }
            return;
        }

        // --- Nothing interactable ---
        promptText.gameObject.SetActive(false);
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

        RaycastHit hit;   // <-- FIX: declared outside the if, now accessible everywhere
        if (Physics.Raycast(ray, out hit, interactionRange, layerMask))
        {
            currentHitName = hit.collider.gameObject.name;
            currentDoor = hit.collider.GetComponent<DoorLevelSelector>();
            currentFridge = hit.collider.GetComponent<FridgeInteract>();   // <-- ADDED
        }
        else
        {
            currentHitName = "";
            currentDoor = null;
            currentFridge = null;
        }
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