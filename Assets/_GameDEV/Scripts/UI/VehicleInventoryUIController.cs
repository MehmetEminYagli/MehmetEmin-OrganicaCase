using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VehicleInventoryUIController : MonoBehaviour
{
    [Header("List References")]
    [SerializeField] private Transform vehicleListContent;
    [SerializeField] private GameObject vehicleItemPrefab;
    [SerializeField] private TextMeshProUGUI totalVehiclesText;

    [Header("Vehicle Details")]
    [SerializeField] private TextMeshProUGUI modelNameText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI topSpeedText;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private Vehicle selectedVehicle;

    public void UpdateVehicleList(List<Vehicle> vehicles)
    {
        // Önceki liste öğelerini temizle
        ClearList();

        foreach (var vehicle in vehicles)
        {
            if (vehicle != null && vehicleItemPrefab != null && vehicleListContent != null)
            {
                GameObject item = Instantiate(vehicleItemPrefab, vehicleListContent);
                var listItemController = item.GetComponent<VehicleListItemController>();
                
                if (listItemController != null)
                {
                    listItemController.Setup(vehicle, this);
                }

                spawnedItems.Add(item);
            }
        }

        // Toplam araç sayısını güncelle
        if (totalVehiclesText != null)
        {
            totalVehiclesText.text = $"Car Count: {vehicles.Count}";
        }

        // İlk aracı seç (eğer varsa)
        if (vehicles.Count > 0)
        {
            SelectVehicle(vehicles[0]);
        }
        else
        {
            ClearVehicleDetails();
        }
    }

    public void SelectVehicle(Vehicle vehicle)
    {
        selectedVehicle = vehicle;
        UpdateVehicleDetails();
    }

    private void UpdateVehicleDetails()
    {
        if (selectedVehicle != null)
        {
            VehicleData data = selectedVehicle.GetVehicleData();
            
            if (modelNameText != null)
                modelNameText.text = data.ModelName;
            
            if (conditionText != null)
                conditionText.text = $"Condition: {data.Condition}%";
            
            if (topSpeedText != null)
                topSpeedText.text = $"Top Speed: {data.TopSpeed} km/h";

            // Liste öğelerinin seçili durumunu güncelle
            UpdateSelectedVisual();
        }
    }

    private void UpdateSelectedVisual()
    {
        // Tüm liste öğelerini kontrol et ve seçili olanı vurgula
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                var listItem = item.GetComponent<VehicleListItemController>();
                var button = item.GetComponent<Button>();
                if (listItem != null && button != null)
                {
                    // Normal rengi veya seçili rengi ayarla
                    ColorBlock colors = button.colors;
                    colors.normalColor = listItem.Vehicle == selectedVehicle ? 
                        new Color(0.7f, 0.7f, 0.7f) : Color.white;
                    button.colors = colors;
                }
            }
        }
    }

    private void ClearVehicleDetails()
    {
        if (modelNameText != null)
            modelNameText.text = "";
        
        if (conditionText != null)
            conditionText.text = "";
        
        if (topSpeedText != null)
            topSpeedText.text = "";
    }

    private void ClearList()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        spawnedItems.Clear();
    }

    private void Update()
    {
        // ESC tuşu ile paneli kapat
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I))
        {
            gameObject.SetActive(false);
        }
    }
} 