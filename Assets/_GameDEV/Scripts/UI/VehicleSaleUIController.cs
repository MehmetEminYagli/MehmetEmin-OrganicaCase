using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VehicleSaleUIController : MonoBehaviour
{
    [Header("List References")]
    [SerializeField] private Transform vehicleListContent;
    [SerializeField] private GameObject vehicleItemPrefab;

    [Header("Vehicle Details")]
    [SerializeField] private TextMeshProUGUI modelNameText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI topSpeedText;
    [SerializeField] private TextMeshProUGUI currentPriceText;

    [Header("Price Settings")]
    [SerializeField] private TMP_InputField newPriceInput;
    [SerializeField] private Button confirmPriceButton;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("References")]
    [SerializeField] private PlayerMoney playerMoney;
    [SerializeField] private VehicleSaleManager saleManager;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private Vehicle selectedVehicle;

    private void Start()
    {
        if (playerMoney == null)
            playerMoney = FindObjectOfType<PlayerMoney>();

        if (saleManager == null)
            saleManager = FindObjectOfType<VehicleSaleManager>();

        if (confirmPriceButton != null)
            confirmPriceButton.onClick.AddListener(OnConfirmPriceClicked);

        if (messageText != null)
            messageText.text = "";
    }

    public void UpdateVehicleList(List<Vehicle> vehicles)
    {
        ClearList();

        foreach (var vehicle in vehicles)
        {
            if (vehicle != null && vehicleItemPrefab != null && vehicleListContent != null)
            {
                GameObject item = Instantiate(vehicleItemPrefab, vehicleListContent);
                var listItemController = item.GetComponent<VehicleListItemController>();
                
                if (listItemController != null)
                {
                    listItemController.Setup(vehicle, null);
                    var button = item.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => SelectVehicleForSale(vehicle));
                    }
                }

                spawnedItems.Add(item);
            }
        }

        // İlk aracı seç (eğer varsa)
        if (vehicles.Count > 0)
        {
            SelectVehicleForSale(vehicles[0]);
        }
        else
        {
            ClearSelection();
        }
    }

    private void SelectVehicleForSale(Vehicle vehicle)
    {
        selectedVehicle = vehicle;
        if (selectedVehicle != null)
        {
            VehicleData data = selectedVehicle.GetVehicleData();
            
            // Araç bilgilerini güncelle
            if (modelNameText != null)
                modelNameText.text = data.ModelName;
            
            if (conditionText != null)
                conditionText.text = $"Condition: {data.Condition}%";
            
            if (topSpeedText != null)
                topSpeedText.text = $"Top Speed: {data.TopSpeed} km/h";

            if (currentPriceText != null)
                currentPriceText.text = $"Car Price: ${data.Price:N2}";

            if (newPriceInput != null)
                newPriceInput.text = data.Price.ToString();

            if (messageText != null)
                messageText.text = "";
        }
    }

    private void OnConfirmPriceClicked()
    {
        if (selectedVehicle == null)
        {
            ShowMessage("Please select a vehicle first!", Color.red);
            return;
        }

        if (float.TryParse(newPriceInput.text, out float newPrice))
        {
            // Yeni fiyatı ayarla
            VehicleData data = selectedVehicle.GetVehicleData();
            data.Price = newPrice;
            
            // UI'ı güncelle
            currentPriceText.text = $"Car Price: ${newPrice:N2}";
            ShowMessage("Price updated successfully!", Color.green);
        }
        else
        {
            ShowMessage("Please enter a valid price!", Color.red);
        }
    }

    private void ShowMessage(string message, Color color)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 3f);
        }
    }

    private void ClearMessage()
    {
        if (messageText != null)
            messageText.text = "";
    }

    private void ClearSelection()
    {
        selectedVehicle = null;
        
        if (modelNameText != null)
            modelNameText.text = "";
        
        if (conditionText != null)
            conditionText.text = "";
        
        if (topSpeedText != null)
            topSpeedText.text = "";

        if (currentPriceText != null)
            currentPriceText.text = "Car Price: $0";

        if (newPriceInput != null)
            newPriceInput.text = "0";

        if (messageText != null)
            messageText.text = "";
    }

    private void ClearList()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }
        spawnedItems.Clear();
    }

    private void Update()
    {
        // ESC tuşu ile paneli kapat
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
} 