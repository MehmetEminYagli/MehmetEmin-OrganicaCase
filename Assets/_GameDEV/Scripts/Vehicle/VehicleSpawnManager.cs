using UnityEngine;
using System.Collections.Generic;

public class VehicleSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public string pointName;
        public Transform spawnTransform;
        public bool isOccupied;
        public BoxCollider triggerArea;
    }

    [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    private Dictionary<Transform, SpawnPoint> spawnPointLookup = new Dictionary<Transform, SpawnPoint>();

    private void Start()
    {
        // Her spawn noktası için trigger oluştur
        foreach (var point in spawnPoints)
        {
            if (point.triggerArea == null)
            {
                // Trigger area yoksa oluştur
                var triggerObject = new GameObject($"TriggerArea_{point.pointName}");
                triggerObject.transform.SetParent(point.spawnTransform);
                triggerObject.transform.localPosition = Vector3.zero;
                
                var collider = triggerObject.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.size = new Vector3(3f, 2f, 5f); // Araç boyutuna göre ayarlayın
                
                var detector = triggerObject.AddComponent<SpawnPointDetector>();
                detector.Initialize(this, point.spawnTransform);
                
                point.triggerArea = collider;
            }

            spawnPointLookup[point.spawnTransform] = point;
        }
    }

    public Transform GetAvailableSpawnPoint()
    {
        foreach (var point in spawnPoints)
        {
            if (!point.isOccupied && !IsSpawnPointBlocked(point))
            {
                point.isOccupied = true;
                return point.spawnTransform;
            }
        }
        
        Debug.LogWarning("Uygun spawn noktası bulunamadı!");
        return null;
    }

    private bool IsSpawnPointBlocked(SpawnPoint point)
    {
        if (point.triggerArea == null) return false;

        // Trigger alanında başka araç var mı kontrol et
        Collider[] colliders = Physics.OverlapBox(
            point.triggerArea.transform.position,
            point.triggerArea.size / 2,
            point.triggerArea.transform.rotation
        );

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<Vehicle>() != null)
            {
                return true;
            }
        }

        return false;
    }

    public void SetSpawnPointOccupied(Transform spawnPoint, bool occupied)
    {
        if (spawnPointLookup.TryGetValue(spawnPoint, out SpawnPoint point))
        {
            point.isOccupied = occupied;
        }
    }

    public void OnVehicleEnterSpawnArea(Transform spawnPoint)
    {
        SetSpawnPointOccupied(spawnPoint, true);
    }

    public void OnVehicleExitSpawnArea(Transform spawnPoint)
    {
        SetSpawnPointOccupied(spawnPoint, false);
    }

    private void OnDrawGizmos()
    {
        // Spawn noktalarını görselleştir
        foreach (var point in spawnPoints)
        {
            if (point.spawnTransform != null)
            {
                Gizmos.color = point.isOccupied ? Color.red : Color.green;
                Gizmos.DrawWireCube(point.spawnTransform.position, new Vector3(3f, 2f, 5f));
            }
        }
    }
} 