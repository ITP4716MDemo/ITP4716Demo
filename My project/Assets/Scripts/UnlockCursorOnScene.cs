using UnityEngine;
using System.Collections;

public class UnlockCursorOnScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ForceUnlock());
    }

    IEnumerator ForceUnlock()
    {
        float endTime = Time.time + 2f;
        while (Time.time < endTime)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            yield return null;
        }
        Debug.Log("Force unlock finished.");
    }
}