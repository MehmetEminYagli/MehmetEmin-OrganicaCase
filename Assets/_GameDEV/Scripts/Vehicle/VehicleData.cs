using UnityEngine;

[CreateAssetMenu(fileName = "New Vehicle", menuName = "Vehicle/Vehicle Data")]
public class VehicleData : ScriptableObject
{
    [Header("Vehicle Information")]
    public string ModelName;
    public float Price;
    public float Condition;
    public float TopSpeed;
    
    [Header("Vehicle Stats")]
    public float acceleration;
    public float handling;
    public float braking;

    [TextArea(3, 5)]
    public string Description;
} 