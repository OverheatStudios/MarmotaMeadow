using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopExitManager : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private GameObject[] m_shopUi;
    [SerializeField] private GameObject m_confirmUi;

    public void ForceLeaveShop()
    {
        SceneManager.LoadScene("NightScene", LoadSceneMode.Single);
    }

    public void TryLeaveShop()
    {
        if (m_inventoryManager.HasItemCategory<Seeds>())
        {
            ForceLeaveShop();
            Debug.Log("Has seeds");
        }
        else
        {
            Debug.Log("No seeds");
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
