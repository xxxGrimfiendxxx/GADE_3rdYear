using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int currentCurrency;

    private void Awake()
    {
        Instance = this;
    }

    public bool SpendCurrency(int amount)
    {
        if (currentCurrency >= amount)
        {
            currentCurrency -= amount;
            return true;
        }
        return false;
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
    }

    public int GetCurrency()
    {
        return currentCurrency;
    }
}
