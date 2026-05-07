using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopExit : MonoBehaviour
{
    [Header("Lobby Settings")]
    public string lobbySceneName = "Lobby";   // Name of your lobby scene
    public GameObject playerPrefab;           // Assign your player prefab (optional)

    public void ReturnToLobby()
    {
        // If you destroyed the player when leaving the lobby, spawn a new one
        if (playerPrefab != null)
        {
            // Check if a player already exists (e.g., from DontDestroyOnLoad)
            if (GameObject.FindGameObjectWithTag("Player") == null)
            {
                Instantiate(playerPrefab);
                Debug.Log("Spawned new player in lobby.");
            }
        }

        // Load the lobby scene
        SceneManager.LoadScene(lobbySceneName);
    }
}