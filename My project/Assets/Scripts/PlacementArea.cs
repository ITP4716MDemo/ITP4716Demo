using UnityEngine;

public class PlacementArea : MonoBehaviour
{
    [Tooltip("Where the vegetable will appear when placed")]
    public Transform placePoint;

    private GameObject placedVeggie = null;

    public bool HasVeggie => placedVeggie != null;

    public void PlaceVeggie(GameObject veggie)
    {
        if (placedVeggie != null)
        {
            Debug.Log("Placement area already has a veggie!");
            return;
        }

        placedVeggie = veggie;
        veggie.transform.SetParent(placePoint);
        veggie.transform.localPosition = Vector3.zero;
        veggie.transform.localRotation = Quaternion.identity;
        veggie.SetActive(true);
    }

    public GameObject RetrieveVeggie()
    {
        GameObject veg = placedVeggie;
        placedVeggie = null;
        return veg;
    }
}