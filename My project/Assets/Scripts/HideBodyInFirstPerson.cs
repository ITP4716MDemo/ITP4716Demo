using UnityEngine;

public class HideBodyInFirstPerson : MonoBehaviour
{
    [Tooltip("Drag the GameObject that contains the player's visible body here")]
    public GameObject bodyModel;

    void Start()
    {
        if (bodyModel != null)
        {
            bodyModel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No body model assigned – please assign it in the Inspector.");
        }
    }
}