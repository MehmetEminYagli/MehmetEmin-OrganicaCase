using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    public static NPCSpawner Instance { get; private set; }

    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int maxNPCs = 5;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Transform spawnCenter;
    [SerializeField] private VehicleInspectionPoint inspectionPoint;

    private List<NPCController> activeNPCs = new List<NPCController>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (spawnCenter == null)
            spawnCenter = transform;

        if (inspectionPoint == null)
        {
            Debug.LogError("VehicleInspectionPoint atanmamış! Lütfen Inspector'dan atayın.");
            return;
        }

        SpawnInitialNPCs();
    }

    private void SpawnInitialNPCs()
    {
        for (int i = 0; i < maxNPCs; i++)
        {
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        Vector3 randomPosition = GetRandomSpawnPosition();
        
        if (TryGetValidPosition(randomPosition, out Vector3 validPosition))
        {
            GameObject npcObject = Instantiate(npcPrefab, validPosition, Quaternion.identity);
            NPCController npcController = npcObject.GetComponent<NPCController>();
            
            if (npcController != null)
            {
                npcController.Initialize(inspectionPoint);
                activeNPCs.Add(npcController);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPosition = spawnCenter.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        return randomPosition;
    }

    private bool TryGetValidPosition(Vector3 position, out Vector3 validPosition)
    {
        validPosition = position;
        UnityEngine.AI.NavMeshHit hit;
        
        if (UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
        {
            validPosition = hit.position;
            return true;
        }
        
        return false;
    }

    public void RemoveNPC(NPCController npc)
    {
        if (activeNPCs.Contains(npc))
        {
            activeNPCs.Remove(npc);
            Destroy(npc.gameObject);
        }
    }
} 