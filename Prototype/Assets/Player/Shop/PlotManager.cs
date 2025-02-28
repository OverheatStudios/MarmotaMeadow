using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlotData
{
    public int numberOfPlots = 1;
}

public class PlotManager : MonoBehaviour
{
    
    [SerializeField] private string m_saveLocation;
    private string filePath;
    [SerializeField] private List<GameObject> m_plots;
    [SerializeField] private PlotData plots;
    [SerializeField] private int numberOfPlots;
    [SerializeField] private GameObject m_plotPrefab;
    [SerializeField] private SaveManager m_saveManager;

    void Start()
    {
        filePath = m_saveManager.GetFilePath(m_saveLocation);
        Load();
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
    
    private void Save()
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

            if (SceneManager.GetActiveScene().name == "Day Scene")
            {
                for (int i = 0; i < numberOfPlots; i++)
                {
                    Instantiate(m_plotPrefab, m_plots[i].transform);
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
}
