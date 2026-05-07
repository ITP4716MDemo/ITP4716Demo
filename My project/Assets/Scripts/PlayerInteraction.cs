using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Pointer")]
    public Transform pointerTransform;

    private DoorLevelSelector currentDoor;
    private FridgeInteract currentFridge;

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

        // Priority 1: Fridge
        if (currentFridge != null)
        {
            if (Input.GetKeyDown(interactKey))
                currentFridge.Interact();
            return;
        }

        // Priority 2: Door
        if (currentDoor != null)
        {
            if (Input.GetKeyDown(interactKey))
                currentDoor.ShowSelectionMenu();
            return;
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
            currentDoor = hit.collider.GetComponent<DoorLevelSelector>();
            currentFridge = hit.collider.GetComponent<FridgeInteract>();
        }
        else
        {
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