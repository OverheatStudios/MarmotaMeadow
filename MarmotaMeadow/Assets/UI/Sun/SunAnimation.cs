using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SunAnimation : MonoBehaviour
{
    [SerializeField] private ScrObjSun m_sun;
    [SerializeField] private float m_animationDuration = 4;
    [SerializeField] private Image m_image;
    [SerializeField] private GameObject m_fadeInObj;
    [SerializeField] private GameObject m_fadeOutObj;
    [SerializeField] private List<Sprite> m_sunRise;
    [SerializeField] private List<Sprite> m_sunSet;
    private float m_secondsAnimating = 0;

    private void Start()
    {
        Assert.IsTrue(m_sunRise.Count > 0);
        Assert.IsTrue(m_sunSet.Count > 0);
        Assert.IsFalse(m_fadeOutObj.activeInHierarchy);
        Assert.IsTrue(m_fadeOutObj);
        Assert.IsTrue(m_fadeInObj);
        Assert.IsTrue(m_fadeInObj.activeInHierarchy);
        m_fadeOutObj.GetComponent<FadeIn>().LoadSceneAfter(m_sun.GameOver ? "GameOverScene" : (m_sun.IsSunset ? "NightScene" : "Day Scene"));
    }

    public void Update()
    {
        //if (m_fadeInObj == null)
        {
            m_secondsAnimating += Time.deltaTime;
        }

        int index = (int)(Mathf.InverseLerp(0, m_animationDuration, m_secondsAnimating) * GetAnimation().Count);
        if (index < GetAnimation().Count)
        {
            m_image.sprite = GetAnimation()[index];
        }
        else
        {
            m_fadeOutObj.SetActive(true);
        }
    }

    private List<Sprite> GetAnimation()
    {
        return m_sun.IsSunset ? m_sunSet : m_sunRise;
    }
}
