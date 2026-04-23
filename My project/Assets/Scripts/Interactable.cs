using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public string interactionPrompt = "Press E to interact";
    public UnityEvent onInteract;

    public void Interact()
    {
        onInteract.Invoke();
    }
}