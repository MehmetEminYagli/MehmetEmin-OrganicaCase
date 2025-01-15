using UnityEngine;
using System.Collections.Generic;

public class VehicleSalePoint : InteractableObject
{
    [SerializeField] private GameObject saleUI;
    [SerializeField] private VehicleSaleManager saleManager;

    private void Start()
    {
        if (saleUI != null)
        {
            saleUI.SetActive(false);
        }

        if (saleManager == null)
        {
            saleManager = FindObjectOfType<VehicleSaleManager>();
        }
    }

    public override void Interact()
    {
        if (saleUI != null)
        {
            // UI'ı aç ve satılık araçları güncelle
            saleUI.SetActive(true);
            UpdateVehiclesForSale();
        }
    }

    private void UpdateVehiclesForSale()
    {
        // UI'ı güncelle
        var saleUIController = saleUI.GetComponent<VehicleSaleUIController>();
        if (saleUIController != null)
        {
            saleUIController.UpdateVehicleList(saleManager.GetVehiclesForSale());
        }
    }

    public override bool CanInteract => true;
} 