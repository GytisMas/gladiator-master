using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    [SerializeField] private string[] m_ButtonNames;
    [SerializeField] private PulseButton[] m_Buttons;

    public PulseButton GetButton(string _name)
    {
        for (int i = 0; i < m_ButtonNames.Length; i++)
        {
            if (_name == m_ButtonNames[i])
                return m_Buttons[i];
        }
        return null;
    }
}
