using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteract : MonoBehaviour
{
    [Header("Scene Settings")]
    public string continueSceneName = "Continue";   // Name of the scene to load
    public float delayBeforeLoad = 0f;              // Optional delay (seconds)

    [Header("Feedback")]
    public AudioClip interactSound;                 // Optional door sound
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;

    private AudioSource audioSource;

    void Start()
    {
        // Ensure AudioSource is present if a sound is assigned
        if (interactSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = soundVolume;
        }
    }

    public void Interact()
    {
        Debug.Log($"Door interacted! Loading scene: {continueSceneName}");

        if (interactSound != null && audioSource != null)
            audioSource.PlayOneShot(interactSound);

        if (delayBeforeLoad > 0f)
            Invoke(nameof(LoadScene), delayBeforeLoad);
        else
            LoadScene();
    }

    private void LoadScene()
    {
        // Optional: check if scene exists in Build Settings
        if (IsSceneInBuildSettings(continueSceneName))
            SceneManager.LoadScene(continueSceneName);
        else
            Debug.LogError($"Scene '{continueSceneName}' not found in Build Settings!");
    }

    // Helper to avoid errors if scene is missing
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (name == sceneName)
                return true;
        }
        return false;
    }
}