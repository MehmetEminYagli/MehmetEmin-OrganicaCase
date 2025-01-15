using UnityEngine;

public class SpawnPointDetector : MonoBehaviour
{
    private VehicleSpawnManager spawnManager;
    private Transform spawnPoint;

    public void Initialize(VehicleSpawnManager manager, Transform point)
    {
        spawnManager = manager;
        spawnPoint = point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Vehicle>() != null)
        {
            spawnManager.OnVehicleEnterSpawnArea(spawnPoint);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Vehicle>() != null)
        {
            spawnManager.OnVehicleExitSpawnArea(spawnPoint);
        }
    }
} 