using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private VehicleData vehicleData;

    public VehicleData GetVehicleData()
    {
        return vehicleData;
    }

    public void SetVehicleData(VehicleData data)
    {
        vehicleData = data;
    }
} 