using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusText : MonoBehaviour
{
    [SerializeField] private ScrObjNumNights m_numNights;
    [SerializeField] private ScrObjGlobalData m_data;
    [SerializeField] private TextMeshProUGUI m_text;

    void Start()
    {
        // day is night + 1
        // night is 0 indexed so +2
        m_text.text = string.Format("Day {0} / {1}\nDebt: {2}", m_data.GetData().GetNightCounterPossiblyNegative() + 2, m_numNights.GetFinalNightPlusOne() + 1, m_numNights.GetMoneyRequired());
    }
}
