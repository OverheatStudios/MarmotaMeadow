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
    [SerializeField] private bool isNight;
    [SerializeField] private Bed bed;

    void Update()
    {
        if (m_debug.IsInfiniteDay()) return;
        
        if (maxTime <= 0 && !isNight && !bed.ReturnIsInBed())
        {
            isNight = true;
            maxTime -= Time.deltaTime;
            text.text = Mathf.Ceil(maxTime).ToString() ;
        }
    }

    public bool ReturnIsNight()
    {
       return isNight; 
    }
}
