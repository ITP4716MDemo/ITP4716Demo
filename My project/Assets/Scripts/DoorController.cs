using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Attach this to a door with a trigger collider.
/// - Opens the door slightly when player approaches.
/// - Closes it when player moves away.
/// - Pressing the interaction key (E) loads a target scene.
/// </summary>
public class DoorController : MonoBehaviour
{
    [Header("Door Animation")]
    [Tooltip("The transform that rotates (e.g. the door panel).")]
    public Transform doorHinge;

    [Tooltip("How many degrees the door opens on approach (e.g. 30–45).")]
    public float openAngle = 40f;

    [Tooltip("Rotation speed (degrees per second).")]
    public float rotateSpeed = 90f;

    [Header("Interaction")]
    [Tooltip("Key that triggers scene loading – matches PlayerInteraction.interactKey.")]
    public KeyCode interactKey = KeyCode.E;

    [Tooltip("Name of the scene to load (must be in Build Settings).")]
    public string targetSceneName = "";

    [Tooltip("Build index alternative (used if targetSceneName is empty).")]
    public int targetSceneBuildIndex = -1;

    [Header("UI Prompt (Optional)")]
    public GameObject promptPanel;          // Panel or text object to show when near door
    public Text promptText;                 // Text component inside that panel

    [Header("Audio (Optional)")]
    public AudioClip openSound;
    public AudioClip closeSound;

    // Private state
    private Quaternion closedRotation;
    private Quaternion openRotationTarget;
    private Quaternion currentTargetRotation;
    private bool isPlayerNear = false;
    private bool isLoading = false;
    private AudioSource audioSource;
    private bool wasPlayingOpenSound = false;

    void Start()
    {
        // Use the door's own transform if no hinge is assigned
        if (doorHinge == null)
            doorHinge = transform;

        // Store initial (closed) rotation and calculate open rotation
        closedRotation = doorHinge.localRotation;
        openRotationTarget = closedRotation * Quaternion.Euler(0, openAngle, 0);
        currentTargetRotation = closedRotation;

        // Setup audio source if needed
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (openSound != null || closeSound != null))
            audioSource = gameObject.AddComponent<AudioSource>();

        // Hide UI prompt initially
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void Update()
    {
        // Smoothly rotate the door towards the current target (open/closed)
        if (doorHinge.localRotation != currentTargetRotation)
        {
            doorHinge.localRotation = Quaternion.RotateTowards(
                doorHinge.localRotation,
                currentTargetRotation,
                rotateSpeed * Time.deltaTime
            );
        }

        // Wait for the rotation to finish before playing open/close sounds again
        bool isRotating = doorHinge.localRotation != currentTargetRotation;
        if (!isRotating && wasPlayingOpenSound)
            wasPlayingOpenSound = false;

        // Interaction: player near door AND press the same key as PlayerInteraction
        if (isPlayerNear && !isLoading && Input.GetKeyDown(interactKey))
        {
            InteractWithDoor();
        }

        // Update UI prompt
        if (promptPanel != null && promptText != null)
        {
            bool showPrompt = isPlayerNear && !isLoading;
            promptPanel.SetActive(showPrompt);
            if (showPrompt)
            {
                string sceneDisplay = !string.IsNullOrEmpty(targetSceneName) ? targetSceneName : "Scene " + targetSceneBuildIndex;
                promptText.text = $"Press {interactKey} to go to {sceneDisplay}";
            }
        }
        else if (promptPanel != null)
        {
            promptPanel.SetActive(isPlayerNear && !isLoading);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            currentTargetRotation = openRotationTarget;

            // Play open sound once
            if (openSound != null && audioSource != null && !wasPlayingOpenSound)
            {
                audioSource.PlayOneShot(openSound);
                wasPlayingOpenSound = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            currentTargetRotation = closedRotation;

            if (closeSound != null && audioSource != null)
                audioSource.PlayOneShot(closeSound);
        }
    }

    /// <summary>
    /// Called when the player interacts with the door (E key press).
    /// Loads the target scene.
    /// </summary>
    private void InteractWithDoor()
    {
        if (isLoading) return;
        isLoading = true;

        // Optionally hide prompt immediately
        if (promptPanel != null)
            promptPanel.SetActive(false);

        // Scene loading logic
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else if (targetSceneBuildIndex >= 0 && targetSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(targetSceneBuildIndex);
        }
        else
        {
            Debug.LogError($"DoorController on {gameObject.name}: No valid target scene specified!");
            isLoading = false;
        }
    }

    // Visualise the trigger area in the editor (if a collider is attached)
    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            if (col is BoxCollider box)
                Gizmos.DrawWireCube(box.center, box.size);
            else if (col is SphereCollider sphere)
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            else if (col is CapsuleCollider capsule)
                Gizmos.DrawWireCube(capsule.center, new Vector3(capsule.radius * 2, capsule.height, capsule.radius * 2));
        }
    }
}