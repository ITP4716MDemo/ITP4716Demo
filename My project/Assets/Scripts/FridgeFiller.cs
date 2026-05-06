using UnityEngine;

public class FridgeFiller : MonoBehaviour
{
    public GameObject[] vegetablePrefabs;
    public Transform spawnArea;          // Must have a BoxCollider
    public int numberOfVeggies = 10;
    public bool randomRotation = true;
    public Vector3 positionOffset = Vector3.zero;
    public float overlapRadius = 0.3f;   // Smaller radius to avoid overlapping
    public int maxAttempts = 100;

    // Optional: assign a tag to your vegetables for overlap detection
    // If empty, it will check for any collider (to avoid overlap with anything)
    public string vegetableTag = "Vegetable";

    void Start()
    {
        FillFridge();
    }

    void FillFridge()
    {
        if (vegetablePrefabs == null || vegetablePrefabs.Length == 0)
        {
            Debug.LogError("No vegetable prefabs assigned to FridgeFiller!");
            return;
        }

        if (spawnArea == null)
        {
            Debug.LogError("Spawn area not assigned!");
            return;
        }

        BoxCollider box = spawnArea.GetComponent<BoxCollider>();
        if (box == null)
        {
            Debug.LogError("Spawn area needs a BoxCollider to define bounds!");
            return;
        }

        Bounds bounds = box.bounds;

        for (int i = 0; i < numberOfVeggies; i++)
        {
            int randomIndex = Random.Range(0, vegetablePrefabs.Length);
            GameObject veggiePrefab = vegetablePrefabs[randomIndex];

            Vector3 randomPos = Vector3.zero;
            bool positionFound = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                randomPos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                if (!OverlapsExisting(randomPos + positionOffset))
                {
                    positionFound = true;
                    break;
                }
            }

            if (!positionFound)
            {
                Debug.LogWarning($"Could not place veggie {i} – fridge may be too crowded.");
                continue;
            }

            GameObject veggie = Instantiate(veggiePrefab, randomPos + positionOffset, Quaternion.identity);
            if (randomRotation)
            {
                veggie.transform.rotation = Random.rotation;
            }
            veggie.transform.parent = transform;

            // Optional: give it the "Vegetable" tag for future reference
            veggie.tag = vegetableTag;
        }
    }

    bool OverlapsExisting(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, overlapRadius);
        foreach (Collider hit in hits)
        {
            // If you want to avoid overlapping only vegetables, check the tag
            if (!string.IsNullOrEmpty(vegetableTag) && hit.CompareTag(vegetableTag))
                return true;

            // Alternatively, to avoid overlapping ANY object (walls, other items), use:
            // return true; // but that's too strict
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnArea != null)
        {
            BoxCollider box = spawnArea.GetComponent<BoxCollider>();
            if (box != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
        }
    }
}