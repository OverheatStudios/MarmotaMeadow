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
    [SerializeField] ScrObjGlobalData scrObjGlobalData;

    void Update()
    {
        if (m_debug.IsInfiniteDay()) return;
        
        if (scrObjGlobalData.GetData().GetNightCounterPossiblyNegative() < 0)
        {
            text.text = "Complete the tutorial First";
        }else if(maxTime > 0 && !isNight && !bed.ReturnIsInBed() && scrObjGlobalData.GetData().GetNightCounterPossiblyNegative() >= 0)
        {
            maxTime -= Time.deltaTime;
            text.text = Mathf.Ceil(maxTime).ToString() ;
        }

        if (maxTime <= 0)
        {
            isNight = true;
        }
    }

    public bool ReturnIsNight()
    {
       return isNight; 
    }
}
