using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public string gameSceneName = "YourGameSceneName"; // type your game scene name here

    public void Restart()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}