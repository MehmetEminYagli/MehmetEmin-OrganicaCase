using UnityEngine;
using System.Collections.Generic;

public class VehicleSaleManager : MonoBehaviour
{
    [System.Serializable]
    public class SalePoint
    {
        public string pointName;
        public Transform saleTransform;
        public bool isOccupied;
        public BoxCollider triggerArea;
        private Vehicle vehicleForSale;

        public Vehicle GetVehicleForSale()
        {
            return vehicleForSale;
        }

        public void SetVehicleForSale(Vehicle vehicle)
        {
            vehicleForSale = vehicle;
            isOccupied = vehicle != null;
        }
    }

    [SerializeField] private List<SalePoint> salePoints = new List<SalePoint>();
    private Dictionary<Transform, SalePoint> salePointLookup = new Dictionary<Transform, SalePoint>();

    private void Start()
    {
        // Her satış noktası için trigger oluştur
        foreach (var point in salePoints)
        {
            if (point.triggerArea == null)
            {
                // Trigger area yoksa oluştur
                var triggerObject = new GameObject($"SaleTrigger_{point.pointName}");
                triggerObject.transform.SetParent(point.saleTransform);
                triggerObject.transform.localPosition = Vector3.zero;
                
                var collider = triggerObject.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.size = new Vector3(3f, 2f, 5f); // Araç boyutuna göre ayarlayın
                
                var detector = triggerObject.AddComponent<SalePointDetector>();
                detector.Initialize(this, point.saleTransform);
                
                point.triggerArea = collider;
            }

            salePointLookup[point.saleTransform] = point;
        }
    }

    // Satılık araçları al
    public List<Vehicle> GetVehiclesForSale()
    {
        List<Vehicle> vehicles = new List<Vehicle>();
        foreach (var point in salePoints)
        {
            if (point.isOccupied && point.GetVehicleForSale() != null)
            {
                vehicles.Add(point.GetVehicleForSale());
            }
        }
        return vehicles;
    }

    // Araç satış noktasına girdiğinde
    public void OnVehicleEnterSalePoint(Transform salePoint, Vehicle vehicle)
    {
        if (salePointLookup.TryGetValue(salePoint, out SalePoint point))
        {
            point.SetVehicleForSale(vehicle);
            Debug.Log($"Araç satış noktasına yerleştirildi: {vehicle.GetVehicleData().ModelName}");
        }
    }

    // Araç satış noktasından çıktığında
    public void OnVehicleExitSalePoint(Transform salePoint)
    {
        if (salePointLookup.TryGetValue(salePoint, out SalePoint point))
        {
            point.SetVehicleForSale(null);
        }
    }

    // Aracı sat ve satış noktasından kaldır
    public void SellVehicle(Vehicle vehicle)
    {
        foreach (var point in salePoints)
        {
            if (point.GetVehicleForSale() == vehicle)
            {
                point.SetVehicleForSale(null);
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Satış noktalarını görselleştir
        foreach (var point in salePoints)
        {
            if (point.saleTransform != null)
            {
                Gizmos.color = point.isOccupied ? Color.yellow : Color.green;
                Gizmos.DrawWireCube(point.saleTransform.position, new Vector3(3f, 2f, 5f));
            }
        }
    }
} 