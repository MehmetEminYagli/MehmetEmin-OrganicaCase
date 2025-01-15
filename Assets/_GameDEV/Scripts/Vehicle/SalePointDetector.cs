using UnityEngine;

public class SalePointDetector : MonoBehaviour
{
    private VehicleSaleManager saleManager;
    private Transform salePoint;

    public void Initialize(VehicleSaleManager manager, Transform point)
    {
        saleManager = manager;
        salePoint = point;
    }

    private void OnTriggerEnter(Collider other)
    {
        var vehicle = other.GetComponent<Vehicle>();
        if (vehicle != null)
        {
            saleManager.OnVehicleEnterSalePoint(salePoint, vehicle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Vehicle>() != null)
        {
            saleManager.OnVehicleExitSalePoint(salePoint);
        }
    }
} 