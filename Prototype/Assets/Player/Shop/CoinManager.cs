using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private float coins;
    // Start is called before the first frame update
    public void IncreaseCoins(float amount)
    {
        coins += amount;
    }

    public void SetCoins(float amount)
    {
        coins = amount;
    }

    public void DecreaseCoins(float amount)
    {
        coins -= amount;
    }

    public float GetCoins()
    {
        return coins;
    }
}
