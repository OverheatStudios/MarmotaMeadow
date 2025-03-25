using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlotData
{
    public int numberOfPlots = 0;
}

public class PlotManager : MonoBehaviour
{
    
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
        filePath = m_saveManager.GetFilePath(m_saveLocation);
        Load();
        if (scrObjGlobalData && scrObjGlobalData.GetData().GetNightCounterPossiblyNegative() >= 0)
        {
            switch (scrObjGlobalData.GetData().CurrentHealth)
            {
                case 100:
                    IncreaseMultiplierOnPlots(3);
                    break;
                case 80:
                    IncreaseMultiplierOnPlots(2);
                    break;
                case 60:
                    IncreaseMultiplierOnPlots(2);
                    break;
                case 40:
                    IncreaseMultiplierOnPlots(2);
                    break;
                case 20:
                    IncreaseMultiplierOnPlots(1);
                    break;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            IncreaseNumberOfPlots();
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

    void IncreaseMultiplierOnPlots(float amount)
    {
        for (int i = 0; i < m_plotsSpawned.Count; i++)
        {
            m_plotsSpawned[i].gameObject.GetComponentInChildren<Plant>().IncreaseMultiplier(amount);
        }
    }
}
