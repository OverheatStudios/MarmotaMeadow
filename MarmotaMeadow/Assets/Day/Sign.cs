using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_normalGroundhogs;
    [SerializeField] private TextMeshProUGUI m_tankGroundhogs;
    [SerializeField] private TextMeshProUGUI m_rangedGroundhogs;
    [SerializeField] private AllGroundhogSpawns m_allSpawns;
    [SerializeField] private ScrObjGlobalData m_data;

    private void Start()
    {
        int normal = m_allSpawns.CountSpawnsOnNight(m_data.GetData().GetNightCounterPossiblyNegative() + 1, GroundhogType.Basic);
        int tank = m_allSpawns.CountSpawnsOnNight(m_data.GetData().GetNightCounterPossiblyNegative() + 1, GroundhogType.Tank);
        int ranged = m_allSpawns.CountSpawnsOnNight(m_data.GetData().GetNightCounterPossiblyNegative() + 1, GroundhogType.Ranged);

        m_normalGroundhogs.text = "" + normal;
        m_tankGroundhogs.text = "" + tank;
        m_rangedGroundhogs.text = "" + ranged;
    }
}
