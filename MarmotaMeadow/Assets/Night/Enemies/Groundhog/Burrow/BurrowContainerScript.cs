using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class BurrowContainerScript : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_data;
    private int m_currentBurrowCount = 0;
    public int CurrentBurrowCount
    {
        get { return m_currentBurrowCount; }
    }

    // Start is called before the first frame update
    void Start()
    {
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
        int numBurrows = 1;
        switch (whatNight)
        {
            case 0:
                numBurrows = 3;
                break;
            case 1:
                numBurrows = 5;
                break;
            case 2:
                numBurrows = 6;
                break;
            case 3:
                numBurrows = 8;
                break;
            case 4:
                numBurrows = 9;
                break;
            case 5:
                numBurrows = 10;
                break;
            case 6:
                numBurrows = 11;
                break;
            case 7:
                numBurrows = 12;
                break;
            case 8:
                numBurrows = 13;
                break;
            default:
                numBurrows = 14;
                break;
        }

        return Mathf.Min(numBurrows, maxBurrowCount);
    }
}
