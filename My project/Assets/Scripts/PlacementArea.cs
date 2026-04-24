using UnityEngine;
using System.Collections.Generic;

public class PlacementArea : MonoBehaviour
{
    [Tooltip("Where vegetables will appear when placed")]
    public Transform placePoint;

    [Tooltip("Maximum number of vegetables allowed")]
    public int maxVeggies = 2;

    [Tooltip("Offset between vegetables")]
    public Vector3 spacing = new Vector3(0.15f, 0f, 0f);

    [Tooltip("Fridge respawner (drag the fridge object here)")]
    public FridgeRespawner fridgeRespawner;

    private List<GameObject> placedVeggies = new List<GameObject>();

    public int VeggieCount => placedVeggies.Count;
    public bool HasVeggie => placedVeggies.Count > 0;
    public bool IsFull => placedVeggies.Count >= maxVeggies;

    public void PlaceVeggie(GameObject veggie)
    {
        if (IsFull)
        {
            Debug.Log("Placement area is full!");
            return;
        }

        placedVeggies.Add(veggie);
        veggie.transform.SetParent(placePoint);
        Vector3 pos = Vector3.zero;
        if (placedVeggies.Count == 2) pos = spacing;
        veggie.transform.localPosition = pos;
        veggie.transform.localRotation = Quaternion.identity;
        veggie.SetActive(true);
    }

    public GameObject RetrieveFirstVeggie()
    {
        if (placedVeggies.Count == 0) return null;
        GameObject veg = placedVeggies[0];
        placedVeggies.RemoveAt(0);
        RepositionVeggies();

        // Respawn a new vegetable in the fridge when one is taken
        if (fridgeRespawner != null)
            fridgeRespawner.RespawnOneVegetable();

        return veg;
    }

    private void RepositionVeggies()
    {
        for (int i = 0; i < placedVeggies.Count; i++)
        {
            Vector3 pos = (i == 1) ? spacing : Vector3.zero;
            placedVeggies[i].transform.localPosition = pos;
        }
    }
}