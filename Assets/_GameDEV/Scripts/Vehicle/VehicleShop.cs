using UnityEngine;
using System.Collections.Generic;

public class VehicleShop : MonoBehaviour, IVehicleShop
{
    [SerializeField] private List<VehicleData> availableVehicles = new List<VehicleData>();
    private PlayerMoney playerMoney;

    private void Start()
    {
        playerMoney = FindObjectOfType<PlayerMoney>();
    }

    public void DisplayVehicleDetails(VehicleData vehicle)
    {
        // Bu metodu interface'den dolayı tutuyoruz ama kullanmıyoruz
    }

    public bool PurchaseVehicle(VehicleData vehicle, int quantity)
    {
        float totalCost = vehicle.Price * quantity;
        
        if (playerMoney != null && playerMoney.HasEnoughMoney(totalCost))
        {
            playerMoney.SpendMoney(totalCost);
            return true;
        }

        return false;
    }

    public VehicleData[] GetAvailableVehicles()
    {
        return availableVehicles.ToArray();
    }
} 