using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopLastDay : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_data;
    [SerializeField] private ScrObjNumNights m_numNights;
    [SerializeField] private TextMeshProUGUI m_text;

    void Start()
    {
        m_text.text = m_text.text.Replace("[debt]", m_numNights.GetMoneyRequired()+"");
        gameObject.SetActive(m_data.GetData().GetNightCounterPossiblyNegative() + 1 >= m_numNights.GetFinalNightPlusOne());
    }
}
