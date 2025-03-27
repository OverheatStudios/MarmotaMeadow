using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UIElements;

public class PlaySaveContainer : MonoBehaviour
{
    private static int NUM_SAVES_PER_PAGE = 3;
    [SerializeField] private RectTransform m_scrollViewContent;
    [SerializeField] private SaveManager m_saveManager;
    [SerializeField] private GameObject m_playSaveButtonPrefab;
    [SerializeField] private float m_ySpacing = 200;
    [SerializeField] private float m_yOffset = 100;
    [SerializeField] private GameObject m_prevPageButton;
    [SerializeField] private GameObject m_nextPageButton;
    [SerializeField] private TextMeshProUGUI m_pageText;

    private int m_page;
    private List<string> m_saves;
    private GameObject[] m_buttons = new GameObject[3];

    private void Start()
    {
        m_saves = m_saveManager.GetPlayableSaves();

        SetPage(0);
    }

    private void SetPage(int page)
    {
        m_page = page = Mathf.Clamp(page, 0, GetMaxPage());

        m_pageText.text = new StringBuilder().Append("Page ").Append(m_page + 1).Append(" / ").Append(GetMaxPage() + 1).ToString();

        // Display save buttons
        for (int i = 0; i < NUM_SAVES_PER_PAGE; ++i)
        {
            if (m_buttons[i] != null)
            {
                Destroy(m_buttons[i]);
            }

            int saveIndex = m_page * 3 + i;
            if (saveIndex >= m_saves.Count) continue;

            GameObject button = Instantiate(m_playSaveButtonPrefab, m_scrollViewContent);
            button.transform.position += Vector3.up * GetYCoordinateOffset(i);

            PlaySave playSave = button.GetComponent<PlaySave>();
            playSave.SetSaveName(m_saves[saveIndex]);

            m_buttons[i] = button;
        }

        // Page buttons
        m_prevPageButton.SetActive(m_page > 0);
        m_nextPageButton.SetActive(m_page < GetMaxPage());
    }

    public void NextPage()
    {
        SetPage(m_page + 1);
    }

    public void PreviousPage()
    {
        SetPage(m_page - 1);
    }

    private int GetMaxPage()
    {
        return Mathf.Max(0, Mathf.CeilToInt(m_saves.Count / (float)NUM_SAVES_PER_PAGE) - 1);
    }

    private float GetYCoordinateOffset(int index)
    {
        Assert.IsTrue(index >= 0);
        Assert.IsTrue(index <= NUM_SAVES_PER_PAGE - 1);
        return ((NUM_SAVES_PER_PAGE - (index + 2))) * m_ySpacing + m_yOffset * Screen.height / 1920.0f;
    }
}
