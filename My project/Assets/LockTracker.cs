using UnityEngine;

public class LockTracker : MonoBehaviour
{
    void Start()
    {
        Debug.Log("LockTracker is active in the Shop scene.");
    }
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Debug.Log("Cursor locked by: " + StackTraceUtility.ExtractStackTrace());
            enabled = false; // stop spamming
        }
    }
}