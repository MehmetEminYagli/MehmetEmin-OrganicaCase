using UnityEngine;
using System.Collections.Generic;

public class VehicleInspectionPoint : MonoBehaviour
{
    [SerializeField] private VehicleSaleManager saleManager;

    private void Start()
    {
        if (saleManager == null)
        {
            saleManager = FindObjectOfType<VehicleSaleManager>();
        }
    }

    public Transform GetRandomSalePoint()
    {
        if (saleManager != null)
        {
            var vehicles = saleManager.GetVehiclesForSale();
            if (vehicles != null && vehicles.Count > 0)
            {
                int randomIndex = Random.Range(0, vehicles.Count);
                return vehicles[randomIndex].transform;
            }
        }
        
        Debug.LogWarning("Satış noktaları bulunamadı!");
        return null;
    }
} 