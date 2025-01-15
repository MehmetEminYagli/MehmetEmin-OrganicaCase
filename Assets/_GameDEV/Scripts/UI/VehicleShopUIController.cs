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

    [Header("Vehicle Details Texts")]
    [SerializeField] private TextMeshProUGUI modelText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI topSpeedText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI messageText;

    private VehicleData currentVehicle;
    private PlayerMoney playerMoney;

    private void Start()
    {
        playerMoney = FindObjectOfType<PlayerMoney>();
        if (playerMoney != null)
        {
            playerMoney.RegisterOnMoneyUpdated(UpdateMoneyDisplay);
        }
        InitializeUI();
        UpdateMoneyDisplay();
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
        vehicleSelector.ClearOptions();
        foreach (var vehicle in vehicleShop.GetAvailableVehicles())
        {
            vehicleSelector.options.Add(new TMP_Dropdown.OptionData(vehicle.ModelName));
        }

        vehicleSelector.onValueChanged.AddListener(OnVehicleSelected);
        quantityInput.text = "1";

        if (vehicleSelector.options.Count > 0)
        {
            OnVehicleSelected(0);
        }
    }

    private void OnVehicleSelected(int index)
    {
        currentVehicle = vehicleShop.GetAvailableVehicles()[index];
        UpdateDetailsDisplay();
    }

    private void UpdateDetailsDisplay()
    {
        if (currentVehicle != null)
        {
            modelText.text = $"Model: {currentVehicle.ModelName}";
            conditionText.text = $"Condition: {currentVehicle.Condition}%";
            topSpeedText.text = $"Top Speed: {currentVehicle.TopSpeed} km/h";
            priceText.text = $"Price: ${currentVehicle.Price:N2}";
        }
    }

    private void UpdateMoneyDisplay()
    {
        if (playerMoney != null)
        {
            moneyText.text = $"Money: ${playerMoney.GetCurrentMoney():N2}";
        }
    }

    public void OnPurchaseButtonClick()
    {
        if (currentVehicle != null && int.TryParse(quantityInput.text, out int quantity))
        {
            bool success = vehicleShop.PurchaseVehicle(currentVehicle, quantity);
            if (success)
            {
                DisplayMessage($"Purchased {quantity}x {currentVehicle.ModelName}");
            }
            else
            {
                DisplayMessage("Not enough money!");
            }
        }
    }

    private void DisplayMessage(string message)
    {
        messageText.text = message;
        Invoke(nameof(ClearMessage), 3f);
    }

    private void ClearMessage()
    {
        messageText.text = "";
    }

    public void ToggleShopPanel()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }
} 