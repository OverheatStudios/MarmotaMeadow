using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlotData
{
    public int numberOfPlots = 0;
}

public class PlotManager : MonoBehaviour
{
    public static int MAX_PLOTS = 5;
    [SerializeField] private string m_saveLocation;
    private string filePath;
    [SerializeField] private List<GameObject> m_plots;
    [SerializeField] private List<GameObject> m_plotsSpawned;
    [SerializeField] private PlotData plots;
    [SerializeField] private int numberOfPlots;
    [SerializeField] private GameObject m_plotPrefab;
    [SerializeField] private SaveManager m_saveManager;
    [SerializeField] private ScrObjGlobalData scrObjGlobalData;

    void Start()
    {
        Assert.IsTrue(m_plots.Count == MAX_PLOTS || m_plots.Count == 0);
        filePath = m_saveManager.GetFilePath(m_saveLocation);
        Load();
        if (scrObjGlobalData && scrObjGlobalData.GetData().GetNightCounterPossiblyNegative() >= 0)
        {
            float howGood = Mathf.InverseLerp(0, 100, scrObjGlobalData.GetData().CurrentHealth);
            IncreaseGrowthTimerOnPlots(Mathf.Lerp(8, 0, howGood));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }
    }
    
    public void Save()
    {
        plots.numberOfPlots = numberOfPlots;
        string json = JsonUtility.ToJson(plots, true);
        File.WriteAllText(filePath, json);
    }

    private void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlotData data = JsonUtility.FromJson<PlotData>(json);
            
            if (data != null) 
            {
                numberOfPlots = data.numberOfPlots;
            }


            if (SceneManager.GetActiveScene().name == "Day Scene" || SceneManager.GetActiveScene().name == "NightScene")
            {
                for (int i = 0; i < numberOfPlots; i++)
                {
                    if(i > m_plots.Count - 1) break;
                    GameObject plot = Instantiate(m_plotPrefab, m_plots[i].transform);
                    m_plotsSpawned.Add(plot);
                }
            }
        }
    }

    public void IncreaseNumberOfPlots()
    {
        numberOfPlots++;
    }

    public void OnDestroy()
    {
        Save();
    }

    void IncreaseGrowthTimerOnPlots(float amount)
    {
        for (int i = 0; i < m_plotsSpawned.Count; i++)
        {
            m_plotsSpawned[i].gameObject.GetComponentInChildren<Plant>().SetGrowTimerOffset(amount);
        }
    }

    public int GetNumberPlotsMinus1()
    {
        return numberOfPlots;
    }
}
