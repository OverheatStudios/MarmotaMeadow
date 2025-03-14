using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UIElements;

public class PlaySaveContainer : MonoBehaviour
{
    [SerializeField] private Transform m_scrollViewContent;
    [SerializeField] private SaveManager m_saveManager;
    [SerializeField] private GameObject m_playSaveButtonPrefab;
    [SerializeField] private Vector3 m_buttonOffset = new Vector3(400, -50, 0);
    [SerializeField] private float m_buttonSeperation = 20;

    private void Start()
    {
        string[] saves = m_saveManager.GetSaves();
        for (int i = 0; i  < saves.Length; ++i)
        {
            GameObject button = Instantiate(m_playSaveButtonPrefab, m_scrollViewContent);
            PlaySave playSave = button.GetComponent<PlaySave>();
            playSave.SetSaveName(saves[i]);

            RectTransform rect = button.GetComponent<RectTransform>();
            button.transform.position += m_buttonOffset + (m_buttonSeperation + Mathf.Abs(rect.sizeDelta.y)) * i * Vector3.down;
        }
    }
}
