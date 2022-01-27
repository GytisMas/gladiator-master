using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrainingSpot : MonoBehaviour
{
    //private static TrainerUI m_activeTrainerUI;

    public Attributes attribute;
    public UnityAction<TrainingSpot> createUI;
    public UnityAction onTrainingStart;
    public UnityAction<string> onSoundClick;
    public UnityAction destroyFighterFromCenter;
    public TrainerData trainerData;
    public bool isUsed = false;

    [Space]
    [SerializeField] private int m_ID;
    [SerializeField] private string m_name;
    [SerializeField] private GameObject m_characterPrefab;
    [SerializeField] private Transform m_characterPosition;
    private TrainerUI m_trainerUI;
    private GameObject m_activeCharacter;
    private Vector3 m_offset = new Vector3(0f, 350f);
    //private bool m_UIIsActive = false;

    public bool IsUsed
    {
        get
        {
            return isUsed;
        }
    }

    public TrainerUI TrainerUI
    {
        set
        {
            m_trainerUI = value;
        }
    }

    public string Name
    {
        get
        {
            return m_name;
        }
    }

    public int ID
    {
        get
        {
            return m_ID;
        }
    }

    public void StartTraining(FighterData _fighter)
    {
        onTrainingStart?.Invoke();
        destroyFighterFromCenter?.Invoke();
        onSoundClick?.Invoke(AudioManager.CARD_CLICK);
        trainerData = new TrainerData(attribute, _fighter.GetAttribute(attribute), ID);
        AddCharacter();
    }

    public void AddCharacter()
    {
        isUsed = true;
        m_activeCharacter = Instantiate(m_characterPrefab, m_characterPosition);
        Fighter _activeFighter = m_activeCharacter.GetComponentInChildren<Fighter>();
        _activeFighter.Training(Fighter.attackAnimation);
    }

    public void StopTraining()
    {
        isUsed = false;
        Destroy(m_activeCharacter);
    }
}
