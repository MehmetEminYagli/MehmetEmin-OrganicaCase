using UnityEngine;
using System.Collections.Generic;

public class PlayerVehicleInventory : MonoBehaviour
{
    private List<Vehicle> ownedVehicles = new List<Vehicle>();
    [SerializeField] private GameObject vehicleInventoryUI;

    private void Start()
    {
        if (vehicleInventoryUI != null)
        {
            vehicleInventoryUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryUI();
        }
    }

    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle != null && !ownedVehicles.Contains(vehicle))
        {
            ownedVehicles.Add(vehicle);
            Debug.Log($"Yeni araç envantere eklendi: {vehicle.GetVehicleData().ModelName}");
        }
    }

    public void RemoveVehicle(Vehicle vehicle)
    {
        if (vehicle != null && ownedVehicles.Contains(vehicle))
        {
            ownedVehicles.Remove(vehicle);
            Debug.Log($"Araç envanterden çıkarıldı: {vehicle.GetVehicleData().ModelName}");
        }
    }

    public List<Vehicle> GetOwnedVehicles()
    {
        return ownedVehicles;
    }

    private void ToggleInventoryUI()
    {
        if (vehicleInventoryUI != null)
        {
            vehicleInventoryUI.SetActive(!vehicleInventoryUI.activeSelf);
            
            if (vehicleInventoryUI.activeSelf)
            {
                UpdateInventoryDisplay();
            }
        }
    }

    private void UpdateInventoryDisplay()
    {
        var display = vehicleInventoryUI.GetComponent<VehicleInventoryUIController>();
        if (display != null)
        {
            display.UpdateVehicleList(ownedVehicles);
        }
    }
} 