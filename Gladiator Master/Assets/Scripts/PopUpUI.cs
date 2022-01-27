using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PopUpUI : UIAnimations
{
    [SerializeField] private TextMeshProUGUI m_text;

    private float m_maxTime = 0f;
    private float m_currentSeconds = 0f;
    private float m_moveSpeed = 1f;
    private bool m_fadeInDone = false;
    private bool m_fadeOut = false;

    public string Text
    {
        set
        {
            m_text.text = value;
        }
    }

    public float MaxTime
    {
        set
        {
            m_maxTime = value;
        }
    }

    public float MoveSpeed
    {
        set
        {
            m_moveSpeed = value;
        }
    }

    public void FadeInDone()
    {
        m_fadeInDone = true;
    }

    private void IncrementPosition()
    {
        Vector3 _newPosition = transform.position;
        _newPosition.y += m_moveSpeed;
        transform.position = _newPosition;
    }

    private void IncrementTimer()
    {
        if (m_fadeInDone)
        {
            m_currentSeconds += Time.deltaTime;
        }
    }

    private void CheckForTimerEnd()
    {
        if (m_currentSeconds >= m_maxTime && !m_fadeOut && m_fadeInDone)
        {
            m_fadeOut = true;
            SetAnimation(false);
        }
    }

    private void Update()
    {
        IncrementPosition();
        IncrementTimer();
        CheckForTimerEnd();
    }
}
