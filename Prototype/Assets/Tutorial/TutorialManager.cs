using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public TriggerBase[] triggers; // Direct references — no tags required
    public TextMeshProUGUI tutorialText;

    private int currentStep = 0;

    void Start()
    {
        ShowStep(currentStep);
    }

    void ShowStep(int stepIndex)
    {
        if (stepIndex < triggers.Length)
        {
            tutorialText.text = triggers[stepIndex].StepText;
            
            if (triggers.Length > stepIndex && triggers[stepIndex])
            {
                triggers[stepIndex].ActivateTrigger();
                triggers[stepIndex].OnTriggerCompleted += AdvanceStep;
            }
        }
        else
        {
            tutorialText.text = "Tutorial Complete!";
        }
    }

    void AdvanceStep()
    {
        currentStep++;
        ShowStep(currentStep);
    }
}