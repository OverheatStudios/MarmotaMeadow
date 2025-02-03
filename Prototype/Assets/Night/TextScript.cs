using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class TextScript : MonoBehaviour
{
    [SerializeField] private DataScriptableObject m_data;
    [SerializeField] private TextMeshProUGUI m_text;

    void Start()
    {
        Assert.IsNotNull(m_data);
    }

    void Update()
    {
        m_text.text = "Groundhogs Killed: " + m_data.groundhogsKilled + " (Spawned: " + m_data.groundhogsSpawned + ")";
    }
}
