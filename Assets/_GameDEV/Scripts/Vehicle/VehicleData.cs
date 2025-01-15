using UnityEngine;

[System.Serializable]
public class VehicleData
{
    public string ModelName;
    public float Condition;
    public float TopSpeed;
    public float Price;
    public GameObject VehiclePrefab;

    public VehicleData(string modelName, float condition, float topSpeed, float price, GameObject vehiclePrefab)
    {
        ModelName = modelName;
        Condition = condition;
        TopSpeed = topSpeed;
        Price = price;
        VehiclePrefab = vehiclePrefab;
    }
} 