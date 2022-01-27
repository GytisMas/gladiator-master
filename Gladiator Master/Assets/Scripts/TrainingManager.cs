using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainingManager : GameManager
{
    private static TrainingSpot m_activeTrainingSpot;
    private static TrainerUI m_activeTrainerUI;

    [SerializeField] private float m_offsetY = 1f;
    [SerializeField] private Transform m_fighterLayout;
    [SerializeField] private List<TrainingSpot> m_spots;
    [SerializeField] private GameObject m_characterPrefab;
    [SerializeField] private GameObject m_trainerUIPrefab;
    private Fighter m_selectedFighterInTraining;

    protected override void Awake()
    {
        base.Awake();
        TrainingSpotEvents();
    }

    private void Start()
    {
        InstantiateFighter();
        CheckForUsedSpots();
    }

    private void CheckForUsedSpots()
    {
        foreach (TrainingSpot _spot in m_spots)
        {
            if (m_usedTrainingSpots.Contains(_spot.ID))
            {
                _spot.trainerData = GetTrainerDataFromSpotID(_spot);
                _spot.AddCharacter();
            }
        }
    }

    private void TrainingSpotEvents()
    {
        foreach(TrainingSpot _spot in m_spots)
        {
            _spot.onSoundClick = m_audioManager.PlaySlot;
            _spot.createUI = CreateUI;
            _spot.destroyFighterFromCenter = DestroyFighter;
            _spot.onTrainingStart = () => m_usedTrainingSpots.Add(_spot.ID);
        }
    }

    private void TrainingSpotClickBehaviour()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit _hit;
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit))
            {
                if (_hit.collider.gameObject.tag == "Training Spot")
                {
                    TrainingSpot _spot = _hit.collider.GetComponent<TrainingSpot>();
                    CreateUI(_spot);
                }
            }
        }
    }

    private void CreateUI(TrainingSpot _spot)
    {
        if (m_activeTrainingSpot != null)
        {
            m_activeTrainerUI.SetAnimation(false);
            if (m_activeTrainingSpot == _spot)
            {
                m_activeTrainingSpot = null;
                m_activeTrainerUI = null;
                return;
            }
        }

        TrainerUI _ui = Instantiate(m_trainerUIPrefab, transform).GetComponent<TrainerUI>();
        Vector3 _pos = Camera.main.WorldToScreenPoint(_spot.transform.position);
        _pos.y += m_offsetY;
        _ui.transform.position = _pos;
        _ui.onTrainingStart = StartTraining;
        _ui.SetInfo(_spot);
        if (_spot.IsUsed)
        {
            _ui.SetUsedSpotInfo(GetFighterNameFromSpot(_spot));
        }

        m_activeTrainingSpot = _spot;
        m_activeTrainerUI = _ui;
    }

    private string GetFighterNameFromSpot(TrainingSpot _spot)
    {
        int _id = _spot.ID;
        for (int i = m_fightersInTraining.Count - 1; i >= 0; i--)
        {
            KeyValuePair<FighterData, TrainerData> _pair = m_fightersInTraining.ElementAt(i);
            if (_pair.Value.ID == _id)
            {
                return _pair.Key.Name;
            }
        }
        return "";
    }
    private TrainerData GetTrainerDataFromSpotID(TrainingSpot _spot)
    {
        int _id = _spot.ID;
        for (int i = m_fightersInTraining.Count - 1; i >= 0; i--)
        {
            KeyValuePair<FighterData, TrainerData> _pair = m_fightersInTraining.ElementAt(i);
            if (_pair.Value.ID == _id)
            {
                return _pair.Value;
            }
        }
        Debug.LogWarning("Returning null at TrainingManager/GetTrainerDataFromSpotID");
        return null;
    }

    public void SetUIEvents(TrainerUI _ui)
    {
        _ui.onTrainingStart = StartTraining;
    }

    protected override void TrainingComplete(KeyValuePair<FighterData, TrainerData> _pair)
    {
        TrainingSpot _spot = FindSpotByTrainingData(_pair.Value);
        _spot.StopTraining();
        if (_spot == m_activeTrainingSpot)
        {
            m_activeTrainerUI.StopTraining();
        }
        base.TrainingComplete(_pair);
        m_selectedFighter = _pair.Key;
        InstantiateFighter();
    }

    private TrainingSpot FindSpotByTrainingData(TrainerData _tData)
    {
        foreach (TrainingSpot _spot in m_spots)
        {
            if (_spot.trainerData == _tData)
            {
                return _spot;
            }
        }
        Debug.LogWarning("Found null at TrainingManager/FindSpotByTrainingData");
        return null;
    }

    private void UpdateTimer(TrainingSpot _spot, TrainerUI _ui)
    {
        string minutes = "";
        string seconds = (int)_spot.trainerData.Timer % 60 < 10 && (int)_spot.trainerData.Timer / 60 > 0
            ? "0" + (int)_spot.trainerData.Timer % 60 + " s"
            : (int)_spot.trainerData.Timer % 60 + " s";
        minutes += (int)_spot.trainerData.Timer / 60 > 0 ? (int)_spot.trainerData.Timer / 60 + ":" : "";
        _ui.TimerText = minutes + seconds;
        _ui.Fill = _spot.trainerData.Timer / _spot.trainerData.StartTimer;

    }

    private void InstantiateFighter()
    {
        if (m_selectedFighter != null)
        {
            m_selectedFighterInTraining = Instantiate(m_characterPrefab, m_fighterLayout)
                .GetComponentInChildren<Fighter>();
            m_selectedFighterInTraining.Idle();
        }
    }

    private void DestroyFighter()
    {
        Destroy(m_selectedFighterInTraining.transform.parent.gameObject);
        m_selectedFighterInTraining = null;
    }

    private void StartTraining()
    {
        if (m_selectedFighterInTraining != null)
        {
            //Debug.Log("ActiveTrainingSpot " + m_activeTrainingSpot);
            m_activeTrainingSpot.StartTraining(m_selectedFighter);
            m_activeTrainerUI.SetUsedSpotInfo(m_selectedFighter.Name);
            AddTrainingFighter(m_activeTrainingSpot.trainerData);
            return;
        }
        MessagePopUp(1f, 0f, "No fighter selected");
    }

    private void UITimer()
    {
        if (m_activeTrainerUI != null && m_activeTrainingSpot.IsUsed)
        {
            UpdateTimer(m_activeTrainingSpot, m_activeTrainerUI);
        }
    }

    protected override void Update()
    {
        base.Update();
        TrainingSpotClickBehaviour();
        UITimer();
    }


}
