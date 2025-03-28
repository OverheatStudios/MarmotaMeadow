using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;

public class BurrowContainerScript : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_data;
    private int m_currentBurrowCount = 0;
    public int CurrentBurrowCount
    {
        get { return m_currentBurrowCount; }
    }

    [Tooltip("Burrow count per night")]
    [SerializeField] private List<int> m_burrowCountPerNight = new();

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(m_burrowCountPerNight.Count > 0);
        EnableBurrows();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Enable the correct amount of burrows for this night
    /// </summary>
    private void EnableBurrows()
    {
        List<GameObject> burrows = new List<GameObject>();
        foreach (Transform t in transform)
        {
            burrows.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }

        int numBurrows = GetBurrowCount(m_data.GetData().GetNightCounter(), burrows.Count);
        for (int i = 0; i < numBurrows; i++)
        {
            burrows[i].SetActive(true);
        }
        m_currentBurrowCount = numBurrows;
    }

    /// <summary>
    /// How many burrows should be active?
    /// </summary>
    /// <param name="whatNight">Night number, 0 indexed</param>
    /// <param name="maxBurrowCount">Maximum number of burrows</param>
    /// <returns>Number of burrows, 1 to (max burrows)</returns>
    private int GetBurrowCount(int whatNight, int maxBurrowCount)
    {
        int numBurrows = m_burrowCountPerNight[Mathf.Min(m_burrowCountPerNight.Count-1, whatNight)];
        return Mathf.Min(numBurrows, maxBurrowCount);
    }
}
