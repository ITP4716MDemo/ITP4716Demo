using UnityEngine;

public class CursorMonitor : MonoBehaviour
{
    void Update()
    {
        // Log the cursor state every second (or when pressing Space)
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"[CursorMonitor] Cursor lock state: {Cursor.lockState}");
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"[CursorMonitor] Current cursor lock state: {Cursor.lockState}");
        }
    }
}