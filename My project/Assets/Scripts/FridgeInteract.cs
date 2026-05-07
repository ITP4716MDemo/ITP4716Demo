using UnityEngine;
using UnityEngine.SceneManagement;

public class FridgeInteract : MonoBehaviour
{
    [Header("Scene to Load")]
    public string targetSceneName = "FridgeShopScene";  // Set this in Inspector

    public void Interact()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("FridgeInteract: targetSceneName is not set! Cannot load scene.");
        }
    }
}