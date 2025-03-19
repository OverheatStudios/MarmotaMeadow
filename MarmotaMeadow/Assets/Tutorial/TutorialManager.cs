using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ToturialData
{
    public bool isFinsihed = false;
    public int step = 0;
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private string m_saveLocation;
    private string filePath;
    public TriggerBase[] triggers;
    public TextMeshProUGUI tutorialText;
    public String textToDisplay;
    
    
    [SerializeField] ToturialData tutorialData;
    [SerializeField] private SaveManager m_saveManager;

    void Start()
    {
        filePath = m_saveManager.GetFilePath(m_saveLocation);
        Load();
        
        if (!tutorialData.isFinsihed)
        {
            ShowStep(tutorialData.step);
        }
    }

    void Update()
    {
        int stepIndex = tutorialData.step;
        if (stepIndex < triggers.Length)
        {
            tutorialText.text = triggers[stepIndex].StepText;
        }
    }

    void ShowStep(int stepIndex)
    {
        if (stepIndex < triggers.Length)
        {
            
            if (triggers.Length > stepIndex && triggers[stepIndex])
            {
                triggers[stepIndex].ActivateTrigger();
                triggers[stepIndex].OnTriggerCompleted += AdvanceStep;
            }
        }
        else
        {
            tutorialText.text = textToDisplay;
            tutorialData.isFinsihed = true;
            Save();
        }
    }

    void AdvanceStep()
    {
        tutorialData.step++;
        ShowStep(tutorialData.step);
    }
    
    public void OnDestroy()
    {
        Save();
    }
    
    public void Save()
    {
        string json = JsonUtility.ToJson(tutorialData, true);
        File.WriteAllText(filePath, json);
    }
    
    private void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ToturialData data = JsonUtility.FromJson<ToturialData>(json);
            
            if (data != null) 
            {
                tutorialData = data;
            }
        }
    }

    public bool ReturnIsTutorialFinished()
    {
        return tutorialData.isFinsihed;
    }

    public ToturialData GetTutorialData()
    {
        return tutorialData;
    }
}