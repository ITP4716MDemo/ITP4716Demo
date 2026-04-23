using UnityEngine;
using UnityEngine.AI; // Required for NavMesh

public class AIPathMovement : MonoBehaviour
{
    public Transform target; // Drag your player or a waypoint here
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            // Tells the agent to calculate a path and move to the target
            agent.SetDestination(target.position);
        }
    }
}