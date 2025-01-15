using UnityEngine;
using System.Collections.Generic;

public class VehicleShop : MonoBehaviour
{
    [System.Serializable]
    public class VehicleEntry
    {
        public VehicleData vehicleData;
        public GameObject vehiclePrefab;
    }

    [SerializeField] private List<VehicleEntry> availableVehicles = new List<VehicleEntry>();
    [SerializeField] private VehicleSpawnManager spawnManager;
    [SerializeField] private PlayerMoney playerMoney;
    [SerializeField] private PlayerVehicleInventory playerInventory;

    private void Start()
    {
        // PlayerMoney referansını kontrol et
        if (playerMoney == null)
        {
            playerMoney = FindObjectOfType<PlayerMoney>();
            if (playerMoney == null)
            {
                Debug.LogError("PlayerMoney bulunamadı!");
            }
        }

        // SpawnManager referansını kontrol et
        if (spawnManager == null)
        {
            spawnManager = FindObjectOfType<VehicleSpawnManager>();
            if (spawnManager == null)
            {
                Debug.LogError("VehicleSpawnManager bulunamadı!");
            }
        }

        // PlayerVehicleInventory referansını kontrol et
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerVehicleInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("PlayerVehicleInventory bulunamadı!");
            }
        }
    }

    public List<VehicleData> GetAvailableVehicles()
    {
        List<VehicleData> vehicles = new List<VehicleData>();
        foreach (var entry in availableVehicles)
        {
            vehicles.Add(entry.vehicleData);
        }
        return vehicles;
    }

    public bool PurchaseVehicle(VehicleData vehicleData, int quantity = 1)
    {
        if (vehicleData == null)
        {
            Debug.LogError("VehicleData null!");
            return false;
        }

        float totalCost = vehicleData.Price * quantity;
        Debug.Log($"Satın alma girişimi - Araç: {vehicleData.ModelName}, Adet: {quantity}, Toplam Maliyet: ${totalCost:N2}");
        
        if (playerMoney == null)
        {
            Debug.LogError("PlayerMoney referansı eksik!");
            return false;
        }

        if (!playerMoney.HasEnoughMoney(totalCost))
        {
            Debug.LogWarning($"Yetersiz bakiye! Gereken: ${totalCost:N2}, Mevcut: ${playerMoney.GetCurrentMoney():N2}");
            return false;
        }

        var vehicleEntry = availableVehicles.Find(v => v.vehicleData == vehicleData);
        if (vehicleEntry == null)
        {
            Debug.LogError($"Araç bulunamadı: {vehicleData.ModelName}");
            return false;
        }

        bool allSpawned = true;
        List<Vehicle> spawnedVehicles = new List<Vehicle>();

        for (int i = 0; i < quantity; i++)
        {
            Transform spawnPoint = spawnManager.GetAvailableSpawnPoint();
            if (spawnPoint != null)
            {
                GameObject vehicleObject = Instantiate(vehicleEntry.vehiclePrefab, 
                    spawnPoint.position, 
                    spawnPoint.rotation);

                var vehicleComponent = vehicleObject.GetComponent<Vehicle>();
                if (vehicleComponent == null)
                {
                    vehicleComponent = vehicleObject.AddComponent<Vehicle>();
                }
                vehicleComponent.SetVehicleData(vehicleData);
                
                // Envantere ekle
                if (playerInventory != null)
                {
                    playerInventory.AddVehicle(vehicleComponent);
                    spawnedVehicles.Add(vehicleComponent);
                }

                Debug.Log($"Araç spawn edildi: {vehicleData.ModelName} (#{i + 1})");
            }
            else
            {
                Debug.LogWarning($"Spawn point bulunamadı! Araç {i + 1} spawn edilemedi.");
                allSpawned = false;
                break;
            }
        }

        if (allSpawned)
        {
            playerMoney.SpendMoney(totalCost);
            Debug.Log($"Satın alma başarılı! Toplam maliyet: ${totalCost:N2}");
            return true;
        }
        else
        {
            // Spawn edilmiş araçları temizle
            foreach (var vehicle in spawnedVehicles)
            {
                if (vehicle != null)
                {
                    if (playerInventory != null)
                    {
                        playerInventory.RemoveVehicle(vehicle);
                    }
                    Destroy(vehicle.gameObject);
                }
            }
        }

        return false;
    }
} 