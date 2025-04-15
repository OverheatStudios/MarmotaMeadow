using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutomaticItemPrice : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private BaseItem item;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private BuyMultiplier m_buyMultiplier;
    
    void Update()
    {
        text.text = "" + item.ReturnBuyCoinsAmount() * m_buyMultiplier.GetCurrentMultiplier();
    }
}
