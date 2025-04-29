using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMenuScript : MonoBehaviour
{
#if UNITY_EDITOR
    public static bool ForceDisableCheats = false;
#else
    public static bool ForceDisableCheats = true;
#endif

    /// <summary>
    /// If true, debug menu can be opened with ctrl+d, else it can't be opened
    /// </summary>
    [SerializeField] private bool m_canOpenDebugMenu = true;
    [SerializeField] private GameObject m_mainDebugPanel;
    [SerializeField] private CursorHandlerScript m_cursorHandler;
    [SerializeField] private ScrObjGameOver m_gameOverReason;
    [SerializeField] private TextMeshProUGUI m_cheatDisableText;

    private bool m_isDebugMenuOpen = false;

    void Start()
    {
        CloseDebugMenu();
    }

    void Update()
    {
        if (!m_canOpenDebugMenu) return;

        bool isCtrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetKeyDown(KeyCode.H) && isCtrlDown)
        {
            if (m_isDebugMenuOpen) CloseDebugMenu();
            else OpenDebugMenu();
        }
    }

    void OpenDebugMenu()
    {
        m_isDebugMenuOpen = true;
        m_cursorHandler.NotifyUiOpen();
        SetActiveRecursive(m_mainDebugPanel, true);
        m_cheatDisableText.gameObject.SetActive(ForceDisableCheats);
    }

    void CloseDebugMenu()
    {
        m_isDebugMenuOpen = false;
        m_cursorHandler.NotifyUiClosed();
        SetActiveRecursive(m_mainDebugPanel, false);
    }

    void SetActiveRecursive(GameObject obj, bool enabled)
    {
        obj.SetActive(enabled);
        foreach (Transform transform in obj.transform)
        {
            SetActiveRecursive(transform.gameObject, enabled);
        }
    }

    public void ButtonLoadNightScene()
    {
        SceneManager.LoadScene("NightScene", LoadSceneMode.Single);
    }

    public void ButtonLoadShopScene()
    {
        SceneManager.LoadScene("Shop", LoadSceneMode.Single);
    }

    public void ButtonLoadDayScene()
    {
        SceneManager.LoadScene("Day Scene", LoadSceneMode.Single);
    }

    public void ButtonLoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }

    public void GameOverDie()
    {
        m_gameOverReason.GameOverReason = ScrObjGameOver.Reason.Died;
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
    }

    public void GameOverDebt()
    {
        m_gameOverReason.GameOverReason = ScrObjGameOver.Reason.Bankrupt;
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
    }

    public void GameOverWin()
    {
        m_gameOverReason.GameOverReason = ScrObjGameOver.Reason.Won;
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
    }
}
