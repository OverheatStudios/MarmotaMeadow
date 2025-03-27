using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebtTooltip : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_data;
    [SerializeField] private TooltipManager m_tooltipManager;
    [SerializeField] private GameObject m_debtTooltip;
    [SerializeField] private float m_tooltipDuration = 8;
    [SerializeField] private float m_tooltipDelay = 3;

    void Start()
    {
        StartCoroutine(ShowTooltip());
    }

    private IEnumerator ShowTooltip()
    {
        yield return new WaitForSeconds(m_tooltipDelay);
        if (m_data.GetData().GetNightCounterPossiblyNegative() == 0)
        {
            m_tooltipManager.ShowTooltip(m_debtTooltip, m_tooltipDuration);
        }
        yield return null;
    }
}
