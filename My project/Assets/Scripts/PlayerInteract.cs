using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Prompt")]
    public Text promptText;

    private InteractableObject currentInteractable;

    private void Update()
    {
        FindInteractable();

        if (currentInteractable != null)
        {
            promptText.text = currentInteractable.interactionPrompt;
            promptText.gameObject.SetActive(true);

            if (Input.GetKeyDown(interactKey))
            {
                currentInteractable.Interact();
            }
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void FindInteractable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        float closestDistance = interactionRange + 1f;
        InteractableObject closest = null;

        foreach (var hitCollider in hitColliders)
        {
            InteractableObject interactable = hitCollider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = interactable;
                }
            }
        }
        currentInteractable = closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}