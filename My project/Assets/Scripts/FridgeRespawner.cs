using UnityEngine;

public class FridgeRespawner : MonoBehaviour
{
    public GameObject[] vegetablePrefabs;
    public Transform spawnArea;       // same as the FridgeSpawnArea transform
    public float overlapRadius = 0.5f;
    public int maxAttempts = 100;

    public void RespawnOneVegetable()
    {
        if (vegetablePrefabs == null || vegetablePrefabs.Length == 0)
        {
            Debug.LogError("No vegetable prefabs assigned to FridgeRespawner!");
            return;
        }

        BoxCollider box = spawnArea?.GetComponent<BoxCollider>();
        if (box == null)
        {
            Debug.LogError("Spawn area needs a BoxCollider!");
            return;
        }

        Bounds bounds = box.bounds;

        // Pick random prefab
        GameObject prefab = vegetablePrefabs[Random.Range(0, vegetablePrefabs.Length)];
        Vector3 spawnPos = Vector3.zero;
        bool found = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 candidate = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (!Physics.CheckSphere(candidate, overlapRadius))
            {
                spawnPos = candidate;
                found = true;
                break;
            }
        }

        if (!found)
            spawnPos = bounds.center;

        Instantiate(prefab, spawnPos, Random.rotation, transform);
    }
}