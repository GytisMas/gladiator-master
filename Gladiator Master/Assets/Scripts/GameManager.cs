using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    protected AudioManager m_audioManager;
    protected Dictionary<FighterData, TrainerData> m_fightersInTraining;
    protected List<int> m_usedTrainingSpots = new List<int>();
    protected FighterContainer m_ownedChampions;
    protected FighterData m_selectedFighter;
    protected int m_fightsWon;
    protected int m_currency;

    [SerializeField]
    private GameObject m_indicatorPrefab;

    protected virtual void Awake()
    {
        LoadData();
    }

    public void FightScene()
    {
        SaveData();
        SceneManager.LoadScene("Fighting");
    }

    public void TrainingScene()
    {
        SaveData();
        SceneManager.LoadScene("Training");
    }

    public void ManagementScene()
    {
        SaveData();
        SceneManager.LoadScene("Management");
    }

    public void AddTrainingFighter(TrainerData _trainerData)
    {
        if (m_fightersInTraining == null)
        {
            m_fightersInTraining = new Dictionary<FighterData, TrainerData>();
        }
        m_fightersInTraining.Add(m_selectedFighter, _trainerData);
        m_ownedChampions.Remove(m_selectedFighter);
        m_selectedFighter = null;
    }

    protected virtual void LoadData()
    {
        m_fightersInTraining = DataHolder.fightersInTraining;
        m_ownedChampions = DataHolder.ownedChampions;
        m_selectedFighter = DataHolder.selectedFighter;
        m_usedTrainingSpots = DataHolder.usedTrainingSpots;
        m_fightsWon = DataHolder.fightsWon;
        m_currency = DataHolder.currency;
    }

    protected virtual void SaveData()
    {
        DataHolder.fightersInTraining = m_fightersInTraining;
        DataHolder.ownedChampions = m_ownedChampions;
        DataHolder.selectedFighter = m_selectedFighter;
        DataHolder.usedTrainingSpots = m_usedTrainingSpots;
        DataHolder.fightsWon = m_fightsWon;
        DataHolder.currency = m_currency;
    }

    protected virtual void TrainingComplete(KeyValuePair<FighterData, TrainerData> _pair)
    {
        Attributes _attribute = _pair.Value.Attribute;
        _pair.Key.FighterStatBoost(_attribute, Random.Range(10, 21));
        MessagePopUp(5f, 0.1f, $"{_pair.Key.Name} now has " +
                            $"{_pair.Key.GetAttribute(_attribute)} {_attribute}!");
        RemoveTrainingFighter(_pair);
        m_audioManager.PlaySlot(AudioManager.CARD_CLICK);
    }

    protected void MessagePopUp(float _maxTime, float _moveSpeed, string _message)
    {
        PopUpUI _indicator = Instantiate(m_indicatorPrefab, transform).GetComponent<PopUpUI>();
        _indicator.MoveSpeed = _moveSpeed;
        _indicator.MaxTime = _maxTime;
        _indicator.Text = _message;
    }

    private void RemoveTrainingFighter(KeyValuePair<FighterData, TrainerData> _pair)
    {
        m_ownedChampions.Add(_pair.Key);
        m_fightersInTraining.Remove(_pair.Key);
        m_usedTrainingSpots.Remove(_pair.Value.ID);
    }

    private void ManageTrainerData()
    {
        if (m_fightersInTraining != null && m_fightersInTraining.Count > 0)
        {
            for (int i = m_fightersInTraining.Count - 1; i >= 0; i--)
            {
                KeyValuePair<FighterData, TrainerData> _pair = m_fightersInTraining.ElementAt(i);
                if (_pair.Value.Timer > 0)
                {
                    _pair.Value.Timer -= Time.deltaTime;
                }
                else
                {
                    TrainingComplete(_pair);
                }
            }
        }
    }

    protected virtual void Update()
    {
        ManageTrainerData();
    }
}
