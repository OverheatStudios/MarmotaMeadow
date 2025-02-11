using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderDefaultValueScript : MonoBehaviour
{
    /// <summary>
    ///  The slider whose value should be set
    /// </summary>
    [SerializeField] private Slider m_slider;
    /// <summary>
    /// The scriptable object with the function
    /// </summary>
    [SerializeField] private ScriptableObject m_scriptableObject;
    /// <summary>
    /// Name of a function in the scriptable object which returns a float and takes no parameters
    /// </summary>
    [SerializeField] private String m_defaultValueGetterFunction;

    // idk how i ended up writing reflection code to make ui but i managed
    void Start()
    {
        Type type = m_scriptableObject.GetType();
        object obj = type.GetMethod(m_defaultValueGetterFunction).Invoke(m_scriptableObject, null);
        Assert.IsTrue(obj is float);
        m_slider.value = (float)obj;
    }
}
