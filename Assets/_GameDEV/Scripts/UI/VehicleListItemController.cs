using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VehicleListItemController : MonoBehaviour
{
    private Vehicle vehicle;
    private VehicleInventoryUIController inventoryUI;
    private Button button;
    private TextMeshProUGUI itemText;

    public Vehicle Vehicle => vehicle;

    private void Awake()
    {
        button = GetComponent<Button>();
        itemText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Setup(Vehicle vehicleData, VehicleInventoryUIController uiController)
    {
        vehicle = vehicleData;
        inventoryUI = uiController;

        if (vehicle != null && itemText != null)
        {
            itemText.text = vehicle.GetVehicleData().ModelName;
        }

   
    }

    public void BTNGetVehicleInfo()
    {
        if (inventoryUI != null && vehicle != null)
        {
            inventoryUI.SelectVehicle(vehicle);
        }
    }
} 