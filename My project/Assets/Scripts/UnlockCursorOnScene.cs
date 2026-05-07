using UnityEngine;

public class UnlockCursorOnScene : MonoBehaviour
{
    void Start()
    {
        // Unlock cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}