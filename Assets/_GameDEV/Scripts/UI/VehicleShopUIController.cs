using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VehicleShopUIController : MonoBehaviour
{
    [SerializeField] private VehicleShop vehicleShop;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TMP_Dropdown vehicleSelector;
    [SerializeField] private TMP_InputField quantityInput;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Vehicle Details Texts")]
    [SerializeField] private TextMeshProUGUI modelText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI topSpeedText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private VehicleData currentVehicle;
    private PlayerMoney playerMoney;

    private void Start()
    {
        // VehicleShop referansını kontrol et
        if (vehicleShop == null)
        {
            vehicleShop = FindObjectOfType<VehicleShop>();
            if (vehicleShop == null)
            {
                Debug.LogError("VehicleShop bulunamadı!");
                return;
            }
        }

        playerMoney = FindObjectOfType<PlayerMoney>();
        if (playerMoney != null)
        {
            playerMoney.RegisterOnMoneyUpdated(UpdateMoneyDisplay);
        }
        else
        {
            Debug.LogError("PlayerMoney bulunamadı!");
        }

        InitializeUI();
        UpdateMoneyDisplay();
        
        // Panel başlangıçta kapalı olsun
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (playerMoney != null)
        {
            playerMoney.UnregisterOnMoneyUpdated(UpdateMoneyDisplay);
        }
    }

    private void InitializeUI()
    {
        if (vehicleSelector != null)
        {
            vehicleSelector.ClearOptions();
            var vehicles = vehicleShop.GetAvailableVehicles();
            foreach (var vehicle in vehicles)
            {
                vehicleSelector.options.Add(new TMP_Dropdown.OptionData(vehicle.ModelName));
            }

            vehicleSelector.onValueChanged.AddListener(OnVehicleSelected);
            
            if (vehicleSelector.options.Count > 0)
            {
                vehicleSelector.value = 0;
                OnVehicleSelected(0);
            }
        }

        if (quantityInput != null)
        {
            quantityInput.text = "1";
        }
    }

    private void OnVehicleSelected(int index)
    {
        var vehicles = vehicleShop.GetAvailableVehicles();
        if (index >= 0 && index < vehicles.Count)
        {
            currentVehicle = vehicles[index];
            UpdateDetailsDisplay();
        }
    }

    private void UpdateDetailsDisplay()
    {
        if (currentVehicle != null)
        {
            if (modelText != null) modelText.text = $"Model: {currentVehicle.ModelName}";
            if (conditionText != null) conditionText.text = $"Condition: {currentVehicle.Condition}%";
            if (topSpeedText != null) topSpeedText.text = $"Top Speed: {currentVehicle.TopSpeed} km/h";
            if (priceText != null) priceText.text = $"Price: ${currentVehicle.Price:N2}";
        }
    }

    private void UpdateMoneyDisplay()
    {
        if (playerMoney != null && moneyText != null)
        {
            moneyText.text = $"Money: ${playerMoney.GetCurrentMoney():N2}";
        }
    }

    public void OnPurchaseButtonClick()
    {
        if (currentVehicle == null)
        {
            DisplayMessage("Lütfen bir araç seçin!");
            return;
        }

        int quantity = 1;
        if (quantityInput != null && !string.IsNullOrEmpty(quantityInput.text))
        {
            if (!int.TryParse(quantityInput.text, out quantity) || quantity < 1)
            {
                DisplayMessage("Geçersiz miktar!");
                return;
            }
        }

        Debug.Log($"Satın alma denemesi - Araç: {currentVehicle.ModelName}, Miktar: {quantity}");
        bool success = vehicleShop.PurchaseVehicle(currentVehicle, quantity);
        
        if (success)
        {
            DisplayMessage($"Başarıyla {quantity}x {currentVehicle.ModelName} satın alındı!");
            UpdateMoneyDisplay();
        }
        else
        {
            if (!playerMoney.HasEnoughMoney(currentVehicle.Price * quantity))
            {
                DisplayMessage($"Yetersiz bakiye! ${currentVehicle.Price * quantity:N2} gerekiyor.");
            }
            else
            {
                DisplayMessage("Satın alma başarısız! Spawn noktası bulunamadı.");
            }
        }
    }

    private void DisplayMessage(string message)
    {
        Debug.Log($"Shop Message: {message}");
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = message.Contains("Başarıyla") ? Color.green : Color.red;
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 3f);
        }
    }

    private void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    public void ToggleShopPanel()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }
    }

    private void Update()
    {
        // ESC tuşu ile paneli kapat
        if (Input.GetKeyDown(KeyCode.Escape) && shopPanel != null && shopPanel.activeSelf)
        {
            shopPanel.SetActive(false);
        }
    }
} 