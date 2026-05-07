using UnityEngine;

public class ConveyorBeltManager : MonoBehaviour
{
    public Transform beltStart;        // The start position of the belt
    public GameObject potPrefab;       // The pot prefab
    public int numberOfPots = 5;
    public float spacing = 2f;

    private void Start()
    {
        // Spawn pots spaced along the belt
        for (int i = 0; i < numberOfPots; i++)
        {
            Vector3 pos = beltStart.position + Vector3.right * i * spacing;
            GameObject pot = Instantiate(potPrefab, pos, Quaternion.identity);
            ConveyorPot cp = pot.GetComponent<ConveyorPot>();
            if (cp != null)
                cp.startPoint = beltStart;
        }
    }
}