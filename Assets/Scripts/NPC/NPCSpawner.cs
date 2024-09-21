using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;         // All spawn points in the area
    [SerializeField] float activeRangeRadius = 20f;   // Radius within which spawners & NPCs are activated
    [SerializeField] int NoOfNpcs = 3;                // Number of NPCs to spawn per Spawnpoint
    [SerializeField] float npcDensityRange = 10f;     // Minimum distance between NPCs at a single spawn point
    [SerializeField] int maxNPCsInArea = 20;          // Maximum number of NPCs allowed in the area

    private Transform playerTransform;                // Reference to the player
    private List<Transform> activeSpawners;            // List of currently active spawn points
    [SerializeField]private List<GameObject> activeNPCs;               // List of currently active NPCs

    private void Awake()
    {
        // Find the player in the scene by tag
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        activeSpawners = new List<Transform>();
        activeNPCs = new List<GameObject>();
    }

    private void Update()
    {
        CheckSpawnPoints();
        HandleSpawning();
        DeactivateDistantNPCs();
    }

    // Check the distance of the player to each spawn point to activate/deactivate spawners
    void CheckSpawnPoints()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            float distanceToPlayer = Vector3.Distance(playerTransform.position, spawnPoint.position);

            if (distanceToPlayer <= activeRangeRadius && !activeSpawners.Contains(spawnPoint))
            {
                // Activate the spawn point if within range and not already active
               // spawnPoint.gameObject.SetActive(true);  // Activate the spawn point object
                activeSpawners.Add(spawnPoint);
            }
            else if (distanceToPlayer > activeRangeRadius && activeSpawners.Contains(spawnPoint))
            {
                // Deactivate the spawn point if beyond range and active
                //activeSpawners.Remove(spawnPoint);
                spawnPoint.gameObject.SetActive(false); // Deactivate the spawn point object
            }
        }
    }

    // Handle spawning for all active spawn points
    private void HandleSpawning()
    {
        // If the number of NPCs in the area exceeds the limit, stop spawning
        if (GetNPCCountInArea() >= maxNPCsInArea)
        {
            return; // Do not spawn more NPCs if the limit is reached
        }

        foreach (Transform spawnPoint in activeSpawners)
        {
            SpawnNPCsAtPoint(spawnPoint);
        }
    }

    // Count the number of NPCs in the area
    private int GetNPCCountInArea()
    {
        int count = 0;
        foreach (GameObject npc in activeNPCs)
        {
            if (npc.activeSelf && Vector3.Distance(playerTransform.position, npc.transform.position) <= activeRangeRadius)
            {
                count++;
            }
        }
        return count;
    }

    // Spawn NPCs at the given spawn point using object pooling
    void SpawnNPCsAtPoint(Transform spawnPoint)
    {
        // Ensure no more than the specified number of NPCs exist near this spawn point
        int existingNpcsAtPoint = 0;

        foreach (GameObject npc in activeNPCs)
        {
            if (npc.activeSelf && Vector3.Distance(npc.transform.position, spawnPoint.position) <= npcDensityRange)
            {
                existingNpcsAtPoint++;
                if (existingNpcsAtPoint >= NoOfNpcs)
                {
                    return;  // Enough NPCs are already near this spawn point, no need to spawn more
                }
            }
        }

        // Spawn new NPCs if below the allowed density
        Spwan(spawnPoint, NoOfNpcs - existingNpcsAtPoint);
    }

    // Spawn the given number of NPCs at the spawn point
    private void Spwan(Transform spawnPoint, int npcToSpawn)
    {
        for (int i = 0; i < npcToSpawn; i++)
        {
            // Get an NPC from the pool
            GameObject _npc = NPC_Pool.Instance.GetPooledNPC();

            if (_npc != null)
            {
                // Set the NPC's position and rotation to the spawn point
                _npc.transform.position = spawnPoint.position;
                _npc.transform.rotation = spawnPoint.rotation;

                // Activate the NPC and add to the list of active NPCs
                _npc.SetActive(true);
                activeNPCs.Add(_npc);
            }
        }
    }

    // Deactivate NPCs that are too far from the player
    void DeactivateDistantNPCs()
    {
        List<GameObject> npcsToDeactivate = new List<GameObject>();

        foreach (GameObject npc in activeNPCs)
        {
            float distanceToPlayer = Vector3.Distance(playerTransform.position, npc.transform.position);

            if (distanceToPlayer > activeRangeRadius)
            {
                npcsToDeactivate.Add(npc);
            }
        }

        // Deactivate NPCs and return them to the pool
        foreach (GameObject npc in npcsToDeactivate)
        {
            npc.SetActive(false);
            NPC_Pool.Instance.ReturnNPCToPool(npc); // Return the NPC to the pool
            activeNPCs.Remove(npc);              // Remove the NPC from the active list
        }
    }
}
