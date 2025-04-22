using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopExitManager : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private GameObject[] m_shopUi;
    [SerializeField] private GameObject m_confirmUi;
    [SerializeField] private ShopFadeIn m_fadeOutUi;
    [SerializeField] private ScrObjSun m_sun;

    public void ForceLeaveShop()
    {
        if (m_fadeOutUi != null && !m_fadeOutUi.gameObject.activeInHierarchy)
        {
            m_fadeOutUi.gameObject.SetActive(true);
            m_sun.IsSunset = true;
            m_fadeOutUi.LoadSceneAfter("SunScene");
        }
    }

    public void TryLeaveShop()
    {
        if (m_inventoryManager.HasItemCategory<Seeds>())
        {
            ForceLeaveShop();
        }
        else
        {
            foreach (GameObject go in m_shopUi)
            {
                go.SetActive(false);
            }
            m_confirmUi.SetActive(true);
        }
    }

    public void CloseConfirmMenu()
    {
        foreach (GameObject go in m_shopUi)
        {
            go.SetActive(true);
        }
        m_confirmUi.SetActive(false);
    }
}
