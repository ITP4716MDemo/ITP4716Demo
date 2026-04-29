using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;      // Pick up
    public KeyCode throwKey = KeyCode.Mouse0;    // Throw with left mouse button

    [Header("UI Prompt")]
    public Text promptText;

    [Header("Holding")]
    public Transform holdPoint;
    private GameObject heldItem = null;

    [Header("Throwing")]
    public float throwForce = 15f;               // Strength of throw
    public AudioClip throwSound;                 // Optional throw sound

    [Header("Pointer")]
    public Transform pointerTransform;

    [Header("Audio")]
    public AudioClip pickupSound;
    private AudioSource audioSource;

    [Header("Respawn")]
    public FridgeRespawner fridgeRespawner;       // For respawning veggies in fridge

    private InteractableObject currentInteractable;
    private FoodItem currentFood;
    private string currentHitName;

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

        // --- Pickup logic: empty handed, looking at something grabbable ---
        if (heldItem == null && currentFood != null && currentFood.isGrabbable)
        {
            promptText.text = "Press E to pick up " + currentFood.foodName;
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(interactKey))
            {
                PickupItem();
                return;
            }
        }
        // --- Throw logic: holding an item ---
        else if (heldItem != null)
        {
            promptText.text = "Press Left Mouse Button to throw";
            promptText.gameObject.SetActive(true);
            if (Input.GetKeyDown(throwKey))
            {
                ThrowHeldItem();
                return;
            }
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

        // Optional: ignore SpawnArea layer if you have one
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

    void PickupItem()
    {
        if (currentFood == null || heldItem != null) return;

        // Use the FoodItem's Pickup method (assumes it returns the GameObject)
        heldItem = currentFood.Pickup();
        if (heldItem != null)
        {
            if (pickupSound != null && audioSource != null)
                audioSource.PlayOneShot(pickupSound);

            // Respawn a new vegetable in the fridge (if applicable)
            if (fridgeRespawner != null)
                fridgeRespawner.RespawnOneVegetable();

            // Attach to hold point and disable physics while held
            heldItem.transform.SetParent(holdPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
            heldItem.SetActive(true);

            // Disable Rigidbody physics while held
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
    }

    void ThrowHeldItem()
    {
        if (heldItem == null) return;

        // Play throw sound if assigned
        if (throwSound != null && audioSource != null)
            audioSource.PlayOneShot(throwSound);

        // Detach from player
        heldItem.transform.SetParent(null);

        // Enable physics
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            // Apply force in the direction the player is looking (camera or pointer forward)
            Vector3 throwDirection = (pointerTransform != null) ? pointerTransform.forward : Camera.main.transform.forward;
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            // Optional: add a little random torque for spinning
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }

        // Clear held reference
        heldItem = null;
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