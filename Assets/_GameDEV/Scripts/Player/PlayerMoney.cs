using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField] private float currentMoney = 1000f;
    private System.Action onMoneyUpdated;

    private void Start()
    {
        Debug.Log($"Başlangıç parası: ${currentMoney:N2}");
    }

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
        bool hasEnough = currentMoney >= amount;
        Debug.Log($"Para kontrolü - Gereken: ${amount:N2}, Mevcut: ${currentMoney:N2}, Yeterli mi: {hasEnough}");
        return hasEnough;
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        Debug.Log($"Para eklendi: +${amount:N2}, Yeni bakiye: ${currentMoney:N2}");
        onMoneyUpdated?.Invoke();
    }

    public void SpendMoney(float amount)
    {
        if (HasEnoughMoney(amount))
        {
            currentMoney -= amount;
            Debug.Log($"Para harcandı: -${amount:N2}, Yeni bakiye: ${currentMoney:N2}");
            onMoneyUpdated?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Yetersiz bakiye! Gereken: ${amount:N2}, Mevcut: ${currentMoney:N2}");
        }
    }

    public float GetCurrentMoney()
    {
        return currentMoney;
    }
} 