using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string interactionPrompt = "Press E to interact";
    public UnityEvent onInteract;

    public void Interact()
    {
        onInteract.Invoke();
    }
}