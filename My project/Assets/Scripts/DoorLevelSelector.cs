using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorLevelSelector : MonoBehaviour
{
    [Header("Scene to load for level selection")]
    public string selectionSceneName = "LevelSelection";

    [Header("Available levels for this door")]
    public LevelOption[] levelOptions;

    [Header("Optional Audio")]
    public AudioSource audioSource;
    public AudioClip interactSound;

    // Static storage to pass data to the next scene
    private static LevelOption[] pendingLevelOptions;

    [System.Serializable]
    public class LevelOption
    {
        public string displayName;   // Text shown on button
        public string sceneName;     // Actual scene to load when selected
    }

    public void ShowSelectionMenu()
    {
        if (audioSource != null && interactSound != null)
            audioSource.PlayOneShot(interactSound);

        // Store the level options for this door
        pendingLevelOptions = levelOptions;

        // Load the level selection scene (additive or single? Single is simpler)
        SceneManager.LoadScene(selectionSceneName);
    }

    // Called by the LevelSelectionManager to retrieve the stored options
    public static LevelOption[] GetPendingLevelOptions()
    {
        LevelOption[] options = pendingLevelOptions;
        pendingLevelOptions = null; // clear after retrieval
        return options;
    }
}