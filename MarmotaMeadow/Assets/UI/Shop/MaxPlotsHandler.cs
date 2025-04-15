using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxPlotsHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_buyButton;
    [SerializeField] private GameObject m_maxPlots;
    [SerializeField] private PlotManager m_plotManager;
    [SerializeField] private GameObject m_price;

    void Update()
    {
        bool maxPlots = m_plotManager.GetNumberPlotsMinus1() >= PlotManager.MAX_PLOTS;
        m_buyButton.SetActive(!maxPlots);
        m_price.SetActive(!maxPlots);
        m_maxPlots.SetActive(maxPlots);
    }
}
