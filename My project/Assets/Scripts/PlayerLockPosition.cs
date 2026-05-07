using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerLockPosition : MonoBehaviour
{
    private FirstPersonController movementScript;

    void Start()
    {
        movementScript = GetComponent<FirstPersonController>();
        if (movementScript == null)
            Debug.LogError("FirstPersonController not found on this GameObject!");
    }

    public void LockPlayer(bool lockMovement)
    {
        if (movementScript != null)
            movementScript.movementLocked = lockMovement;
    }
}