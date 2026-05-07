using UnityEngine;
using UnityEngine.SceneManagement;

public class FridgeInteract : MonoBehaviour
{
    [Header("Scene to Load")]
    public string targetSceneName = "Shop";

    public void Interact()
    {
        // Find and destroy the player (so it doesn't persist into the shop scene)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("Destroying player before loading shop.");
            Destroy(player);
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' not found.");
        }

        // Load the shop scene
        if (!string.IsNullOrEmpty(targetSceneName))
            SceneManager.LoadScene(targetSceneName);
        else
            Debug.LogError("targetSceneName is not set!");
    }
}