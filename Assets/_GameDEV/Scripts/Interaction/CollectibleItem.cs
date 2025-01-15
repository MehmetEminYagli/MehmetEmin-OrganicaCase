using UnityEngine;

public class CollectibleItem : InteractableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private int itemValue;

    public override void Interact()
    {
        Debug.Log($"Collected {itemName} worth {itemValue} points!");
        // Burada envanter sistemine ekleme yapılabilir
        
        OnInteractionComplete();
        gameObject.SetActive(false);
    }
} 