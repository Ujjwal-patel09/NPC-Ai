using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Pool : MonoBehaviour
{
    [SerializeField]private GameObject[] npcPrefabs;         // Array of NPC prefabs to use for pooling
    [SerializeField]private int poolSize = 20;               // Number of NPCs in the pool
    [HideInInspector]public List<GameObject> npcPool;       // The actual pool of NPCs
    private GameObject NPCPool;
    
    public static NPC_Pool Instance;
    private void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
        }
        
        NPCPool = GameObject.FindGameObjectWithTag("NPCPool");
    }

    void Start()
    {
        // Initialize the pool with inactive NPCs
        npcPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            // Choose a random NPC prefab
            GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
            GameObject npc = Instantiate(npcPrefab);
            npc.transform.SetParent(NPCPool.transform);

            // Deactivate the NPC and add it to the pool
            npc.SetActive(false);
            npcPool.Add(npc);
        }
    }

    // Method to get an inactive NPC from the pool
    public GameObject GetPooledNPC()
    {
        foreach (GameObject npc in npcPool)
        {
            if (!npc.activeInHierarchy) // If the NPC is inactive, use it
            {
                return npc;
            }
        }

        // Optional: Expand pool if needed
        // If all NPCs are in use, you can either create more or return null
        GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
        GameObject newNpc = Instantiate(npcPrefab);
        newNpc.SetActive(false);
        npcPool.Add(newNpc);

        return newNpc;
    }

    // Method to return an NPC back to the pool
    public void ReturnNPCToPool(GameObject npc)
    {
        npc.SetActive(false);
    }
}
