using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;

public class BurrowContainerScript : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_data;
    private int m_currentBurrowCount = 0;

    [Tooltip("Burrow count per night")]
    [SerializeField] private List<int> m_burrowCountPerNight = new();

    void Start()
    {
        Assert.IsTrue(m_burrowCountPerNight.Count > 0);
        EnableBurrows();
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

        int numBurrows = GetBurrowCount(m_data.GetData().GetNightCounterPossiblyNegative() + 1, burrows.Count);
        if (numBurrows <= 0)
        {
            Debug.LogWarning("Burrows for night " + (m_data.GetData().GetNightCounterPossiblyNegative() + 1) + " was " + numBurrows + " (clamped to 1)");
            numBurrows = 1;
        }

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
        int numBurrows = m_burrowCountPerNight[Mathf.Min(m_burrowCountPerNight.Count - 1, whatNight)];
        return Mathf.Min(numBurrows, maxBurrowCount);
    }
}
