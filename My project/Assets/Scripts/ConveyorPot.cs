using UnityEngine;

public class ConveyorPot : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;          // Movement speed along the belt
    public Vector3 moveDirection = Vector3.right;   // Direction of belt (e.g., right, forward)

    [Header("Respawn")]
    public Transform startPoint;       // Assign start position in Inspector
    public string endTag = "ConveyorEnd";  // Tag of the end trigger object

    private void Update()
    {
        // Move the pot continuously
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // When pot hits the end trigger, respawn at start
        if (other.CompareTag(endTag))
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        if (startPoint != null)
            transform.position = startPoint.position;
        else
            Debug.LogWarning("Start point not assigned on " + gameObject.name);
    }
}