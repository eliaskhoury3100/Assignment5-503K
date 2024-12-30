using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AstronautController : MonoBehaviour
{

    [SerializeField] private Transform[] wayPoints;  // Assign waypoints in the Unity editor
    private NavMeshAgent agent;
    private int currentDestinationPoint; // index

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GotoNextWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        Patrolling();
    }

    void GotoNextWaypoint()
    {
        if (wayPoints.Length == 0)
            return;

        // Choose a random waypoint
        currentDestinationPoint = Random.Range(0, wayPoints.Length);

        // Set the destination of the agent to the selected random waypoint
        agent.SetDestination(wayPoints[currentDestinationPoint].position);
    }

    private void Patrolling()
    {
        // Check if the agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            // Go to the next random waypoint
            GotoNextWaypoint();
        }
    }
}
