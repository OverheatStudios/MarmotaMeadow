using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements.Experimental;

public class TutorialShakeEffect : MonoBehaviour
{
    [SerializeField] private RectTransform m_rectTransform;
    [Tooltip("How many seconds between the end of one shake and the start of another")]
    [SerializeField] private float m_secondsBetweenShakes = 5;
    [Tooltip("How long to shake for")]
    [SerializeField] private float m_shakeDuration = 1.5f;
    [Tooltip("Offset per velocity change is intensity * velocityChangeInterval pixels")]
    [SerializeField] private float m_minShakeIntensity = 20.0f;
    [SerializeField] private float m_maxShakeIntensity = 40.0f;
    [SerializeField] private float m_secondsToGoBackToOrigin = 0.08f;
    [SerializeField] private float m_velocityChangeInterval = 0.1f;
    private Vector3 m_currentShakeOffset = Vector3.zero;
    private float m_secondsUntilShakingBegins = 0;
    private float m_shakeSecondsRemaining = 0;
    private Vector3 m_currentVelocity = Vector3.zero;
    private float m_secondsTillVelocityChange = 0;
    private float m_unitsMovedWithAverageShakeIntensitySquared;

    void Start()
    {
        Assert.IsTrue(m_maxShakeIntensity >= m_minShakeIntensity);
        Assert.IsTrue(m_minShakeIntensity >= m_velocityChangeInterval);
        Assert.IsTrue(m_velocityChangeInterval > 0);
        m_secondsUntilShakingBegins = m_secondsBetweenShakes;
        m_unitsMovedWithAverageShakeIntensitySquared = Mathf.Lerp(m_minShakeIntensity, m_maxShakeIntensity, 0.5f) * m_velocityChangeInterval;
        m_unitsMovedWithAverageShakeIntensitySquared *= m_unitsMovedWithAverageShakeIntensitySquared;
    }

    void Update()
    {
        m_secondsUntilShakingBegins -= Time.deltaTime;
        if (m_secondsUntilShakingBegins > 0) return;

        if (m_shakeSecondsRemaining <= 0)
        {
            StartShaking();
        }

        // Shake
        m_shakeSecondsRemaining -= Time.deltaTime;
        Shake();

        // We're done
        if (m_shakeSecondsRemaining <= 0)
        {
            SetNewOffset(Vector3.zero);
            m_secondsUntilShakingBegins = m_secondsBetweenShakes;
        }
    }

    private void StartShaking()
    {
        m_shakeSecondsRemaining = m_shakeDuration;
        m_secondsTillVelocityChange = 0;
    }

    private void Shake()
    {
        // Handle velocity change
        m_secondsTillVelocityChange -= Time.deltaTime;
        if (m_secondsTillVelocityChange <= 0)
        {
            m_secondsTillVelocityChange = m_velocityChangeInterval;

            bool isShakingEnding = m_shakeSecondsRemaining <= m_velocityChangeInterval;
            bool isOffsetLarge = m_currentShakeOffset.sqrMagnitude > m_unitsMovedWithAverageShakeIntensitySquared;
            if (isShakingEnding || isOffsetLarge)
            {
                SetVelocityToGoBackToOrigin();
            }
            else
            {
                PickRandomVelocity();
            }
        }

        // Apply velocity
        SetNewOffset(m_currentShakeOffset + m_currentVelocity * Time.deltaTime);
    }

    private void SetVelocityToGoBackToOrigin()
    {
        m_currentVelocity = -(m_currentShakeOffset / m_velocityChangeInterval); // This velocity will bring us almost (within like 0.001) exactly back to original point
    }

    private void PickRandomVelocity()
    {
        m_currentVelocity = new Vector3(
               Random.Range(0.0f, 1.0f),
               Random.Range(0.0f, 1.0f),
               0.0f
               );
        m_currentVelocity.Normalize();
        m_currentVelocity *= Random.Range(m_minShakeIntensity, m_maxShakeIntensity);
    }

    private void SetNewOffset(Vector3 offset)
    {
        m_rectTransform.position -= m_currentShakeOffset;
        m_currentShakeOffset = offset;
        m_rectTransform.position += offset;
    }
}
