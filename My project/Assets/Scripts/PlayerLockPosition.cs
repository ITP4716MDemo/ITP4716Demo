using UnityEngine;

public class PlayerLockPosition : MonoBehaviour
{
    private CharacterController controller;
    private MonoBehaviour[] scriptsToDisable; // movement, camera look, etc.

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Disable any script that moves the player (like PlayerMovement, MouseLook)
        scriptsToDisable = GetComponents<MonoBehaviour>();
    }

    public void LockPlayer(bool lockMovement)
    {
        if (controller != null)
            controller.enabled = !lockMovement;

        foreach (var script in scriptsToDisable)
        {
            // Don't disable this script itself, and don't disable PlayerInteraction if you have it
            if (script != this && script.GetType().Name != "PlayerInteraction")
                script.enabled = !lockMovement;
        }

        // Also freeze rigidbody if present
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = lockMovement;
    }
}