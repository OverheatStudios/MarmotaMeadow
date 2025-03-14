using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToNight : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float maxTime;
    [SerializeField] private DayDebugMenu m_debug;

    void Update()
    {
        if (m_debug.IsInfiniteDay()) return;

        maxTime -= Time.deltaTime;
        text.text = Mathf.Ceil(maxTime).ToString() ;
        if (maxTime <= 0)
        {
            SceneManager.LoadScene("Shop");
        }
    }
}
