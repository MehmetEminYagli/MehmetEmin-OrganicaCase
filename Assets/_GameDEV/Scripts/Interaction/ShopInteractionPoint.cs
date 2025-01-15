using UnityEngine;

public class ShopInteractionPoint : InteractableObject
{
    [SerializeField] private VehicleShopUIController shopUI;

    public override void Interact()
    {
        if (shopUI != null)
        {
            shopUI.ToggleShopPanel();
        }
        else
        {
            Debug.LogWarning("Shop UI Controller is not assigned to the ShopInteractionPoint!");
        }
    }

    public override bool CanInteract => true;
} 