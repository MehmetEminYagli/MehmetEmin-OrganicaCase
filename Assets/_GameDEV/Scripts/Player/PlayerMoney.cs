using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField] private float currentMoney = 1000f;
    private System.Action onMoneyUpdated;

    public void RegisterOnMoneyUpdated(System.Action callback)
    {
        onMoneyUpdated += callback;
    }

    public void UnregisterOnMoneyUpdated(System.Action callback)
    {
        onMoneyUpdated -= callback;
    }

    public bool HasEnoughMoney(float amount)
    {
        return currentMoney >= amount;
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        onMoneyUpdated?.Invoke();
    }

    public void SpendMoney(float amount)
    {
        if (HasEnoughMoney(amount))
        {
            currentMoney -= amount;
            onMoneyUpdated?.Invoke();
        }
    }

    public float GetCurrentMoney()
    {
        return currentMoney;
    }
} 