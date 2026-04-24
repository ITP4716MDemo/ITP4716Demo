using UnityEngine;
using UnityEngine.UI;

public class PlayerChopping : MonoBehaviour
{
    public float interactionRange = 2f;
    public KeyCode chopKey = KeyCode.E;
    public Text promptText;

    private FoodItem currentFood;

    private void Update()
    {
        FindNearbyFood();

        if (currentFood != null)
        {
            promptText.text = "Press " + chopKey + " to chop " + currentFood.foodName;
            promptText.gameObject.SetActive(true);

            if (Input.GetKeyDown(chopKey))
            {
                currentFood.Chop();
                currentFood = null;
            }
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void FindNearbyFood()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        float closestDist = interactionRange + 1f;
        FoodItem closest = null;

        foreach (Collider col in hitColliders)
        {
            FoodItem food = col.GetComponent<FoodItem>();
            if (food != null && !food.isChopped)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = food;
                }
            }
        }
        currentFood = closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}