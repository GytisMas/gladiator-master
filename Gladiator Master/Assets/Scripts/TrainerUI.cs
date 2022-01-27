using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class TrainerUI : UIAnimations
{
    private const string M_TRAINER_BUSY = "Used by: ";
    private const string M_TRAINER_NOT_BUSY = "Unoccupied";

    public UnityAction onTrainingStart;
    public UnityAction onTrainingStop;
    public UnityAction<TextMeshProUGUI, Image> onTimerUpdate;

    [SerializeField] TextMeshProUGUI m_name;
    [SerializeField] TextMeshProUGUI m_attribute;
    [SerializeField] TextMeshProUGUI m_occupation;
    [SerializeField] PulseButton m_start;
    [SerializeField] Image m_timer;
    [SerializeField] Image m_timerBackground;
    [SerializeField] TextMeshProUGUI m_timerText;

    private bool m_inTraining = false;
    private bool m_attributesSet = false;

    public string Occupation
    {
        set
        {
            m_occupation.text = value;
        }
    }

    public string Attribute
    {
        set
        {
            m_attribute.text = "Trains: " + value;
        }
    }

    public string TimerText
    {
        set
        {
            m_timerText.text = value;
        }
    }

    public float Fill
    {
        set
        {
            m_timer.fillAmount = value;
        }
    }

    private void Awake()
    {
        Occupation = M_TRAINER_NOT_BUSY;
        m_timerBackground.gameObject.SetActive(false);
    }

    public void SetInfo(TrainingSpot _spot)
    {
        if (!m_attributesSet)
        {
            Attribute = _spot.attribute.ToString();
            m_name.text = _spot.Name;
            m_attributesSet = true;
        }
    }

    public void StartTraining()
    {
        onTrainingStart?.Invoke();
    }

    public void StopTraining()
    {
        Occupation = M_TRAINER_NOT_BUSY;
        m_start.gameObject.SetActive(true);
        m_timerBackground.gameObject.SetActive(false);
        m_inTraining = false;
    }

    public void SetUsedSpotInfo(string _name)
    {
        m_start.gameObject.SetActive(false);
        m_timerBackground.gameObject.SetActive(true);
        m_inTraining = true;
        Occupation = M_TRAINER_BUSY + _name;
    }

    private void Update()
    {
        if (m_inTraining)
        {
            onTimerUpdate?.Invoke(m_timerText, m_timer);
        }
    }
}
