using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    private GameObject waypointParent;         // Parent GameObject that holds all the waypoints
    public float detectionRange = 20f;        // Range to detect waypoints
    public Transform[] waypoints;   // Array to hold the waypoints
    private NavMeshAgent agent;

    private void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();
        waypointParent = GameObject.FindGameObjectWithTag("SpawnPoints");
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find all waypoints that are children of the waypoint parent
        waypoints = waypointParent.GetComponentsInChildren<Transform>();

        MoveToRandomWaypoint();
    }

    void MoveToRandomWaypoint()
    {
        List<Transform> validWaypoints = GetWaypointsInRange();

        if (validWaypoints.Count == 0)
        {
            Debug.LogWarning("No valid waypoints within range.");
            return;
        }

        // Choose a random waypoint from valid waypoints
        int randomIndex = Random.Range(0, validWaypoints.Count);
        agent.SetDestination(validWaypoints[randomIndex].position);
    }

    List<Transform> GetWaypointsInRange()
    {
        List<Transform> waypointsInRange = new List<Transform>();

        foreach (Transform waypoint in waypoints)
        {
            if (waypoint.gameObject.activeSelf) // Check if waypoint is active
            {
                float distance = Vector3.Distance(transform.position, waypoint.position);
                if (distance <= detectionRange) // Check if waypoint is within range
                {
                    waypointsInRange.Add(waypoint);
                }
            }
        }

        return waypointsInRange;
    }

    void Update()
    {
        // Check if the NPC has reached the current waypoint
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            MoveToRandomWaypoint();
        }
    }
}
