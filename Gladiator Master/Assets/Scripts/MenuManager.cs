using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : GameManager
{
    private const string M_FIGHTBUTTON = "Fight", M_BUYBUTTON = "Buy", M_SELLBUTTON = "Sell", M_TRAINBUTTON = "Train";
    private const string M_NOOWNEDCHAMPIONS = "There are no fighters stored. Purchase some by pressing the button below.";
    private const string M_CHAMPIONSINTRAINING = "Wait for your champion(s) to finish training or purchase some by pressing the button below.";

    private static UIAnimations m_championMenuButton;

    [SerializeField] private TextMeshProUGUI m_noOwnedChampionsText;
    [SerializeField] private Transform[] m_championManagementButtonHolders;
    [SerializeField] private UIFighterDataDisplay m_unownedDisplay;
    [SerializeField] private UIFighterDataDisplay m_ownedDisplay;
    [SerializeField] private GameObject m_ownedChampInteractionBtnsPrefab;
    [SerializeField] private GameObject m_unownedChampInteractionBtnsPrefab;
    [SerializeField] private TextMeshProUGUI m_coins;
    [SerializeField] private TextMeshProUGUI m_fightWins;
    [Space]
    [SerializeField] private TextAsset m_unownedChampionData;
    [SerializeField] private GameObject m_cardPrefab;
    private GameObject m_usedInteractionButtons;
    private float[] m_championManagementButtonPositions;
    private UIFighterDataDisplay m_activeDisplay;
    private FighterContainer m_activeContainer;
    private FighterContainer m_unownedChampions;
    private FighterCard m_selectedCard;
    private FighterData m_selectedChampion;
    private bool m_startOfSession;

    //protected override void Awake()
    //{
    //    base.Awake();
    //}

    private void Start()
    {
        m_championManagementButtonPositions = new float[m_championManagementButtonHolders.Length];
        if (m_startOfSession == true)
        {
            m_startOfSession = false;
            UpdateCoins(DataHolder.DEFAULT_CURRENCY);
        }
        else
        {
            UpdateCoins(m_currency);
        }
        LoadChampionData();
        SetSelectedFighter();
        OwnedMenu();
        UpdateWins();
    }

    public void UnownedMenu()
    {
        ActiveDisplay(m_unownedDisplay, m_unownedChampions);
        m_usedInteractionButtons = m_unownedChampInteractionBtnsPrefab;
    }

    public void OwnedMenu()
    {
        ActiveDisplay(m_ownedDisplay, m_ownedChampions);
        m_usedInteractionButtons = m_ownedChampInteractionBtnsPrefab;
    }

    public void GoToFightScene()
    {
        if (m_selectedChampion != null)
        {
            FightScene();
        }
        else
        {
            Debug.LogWarning("Cannot go to fight scene, no champion chosen");
        }
    }

    public void GoToTrainingScene()
    {
        if (m_selectedFighter != null)
        {
            TrainingScene();
        }
        else
        {
            Debug.LogWarning("Cannot go to training scene, no champion chosen");
        }
    }

    public void BuyChampion()
    {
        if (m_currency >= m_selectedFighter.Price)
        {
            UpdateCoins(m_currency - m_selectedFighter.Price);
            MessagePopUp(1f, 0f, $"You have bought {m_selectedFighter.Name}!");
            SwapFighterContainer(m_ownedChampions, m_unownedChampions, m_selectedFighter);
        }
        else
        {
            MessagePopUp(0.5f, 0f, $"Not enough funds to buy champion!");
        }
    }

    public void SellChampion()
    {
        UpdateCoins(m_currency + (int)(m_selectedFighter.Price * DataHolder.SELL_RATIO));
        MessagePopUp(1f, 0f, $"You have sold {m_selectedFighter.Name}!");
        SwapFighterContainer(m_unownedChampions, m_ownedChampions, m_selectedFighter);
    }

    public void ClearSelection()
    {
        m_selectedFighter = null;
        m_selectedChampion = null;
        m_selectedCard?.OnClickUINull();
        DisplayChampionManageButtons(false, new Vector2());
    }

    public void PlaySound()
    {
        //Debug.Log("Sound in Management/PlaySound");
        m_audioManager.PlaySlot(AudioManager.CARD_CLICK);
    }

    public void ResetWins()
    {
        m_fightsWon = 0;
        UpdateWins();
    }

    protected override void LoadData()
    {
        base.LoadData();
        m_startOfSession = DataHolder.startOfSession;
        m_unownedChampions = DataHolder.unownedChampions;
    }

    protected override void SaveData()
    {
        base.SaveData();
        DataHolder.startOfSession = m_startOfSession;
        DataHolder.unownedChampions = m_unownedChampions;
    }

    private void AddListenersToInteractionButtons(InteractionButton _buttons, bool _isOwnedMenu)
    {
        if (_isOwnedMenu)
        {
            _buttons.GetButton(M_FIGHTBUTTON).onClicked.AddListener(() => GoToFightScene());
            _buttons.GetButton(M_TRAINBUTTON).onClicked.AddListener(() => GoToTrainingScene());
            _buttons.GetButton(M_SELLBUTTON).onClicked.AddListener(() => SellChampion());
            _buttons.GetButton(M_SELLBUTTON).onClicked.AddListener(() => PlaySound());
        }
        else
        {
            _buttons.GetButton(M_BUYBUTTON).onClicked.AddListener(() => BuyChampion());
            _buttons.GetButton(M_BUYBUTTON).onClicked.AddListener(() => PlaySound());
        }
        //InteractionButton _ownedInteraction = m_ownedChampInteractionBtnsPrefab.GetComponent<InteractionButton>();
        //_ownedInteraction.GetButton(M_FIGHTBUTTON).onClicked.AddListener(() => GoToFightScene());
        //_ownedInteraction.GetButton(M_TRAINBUTTON).onClicked.AddListener(() => GoToTrainingScene());
        //_ownedInteraction.GetButton(M_SELLBUTTON).onClicked.AddListener(() => SellChampion());
        //_ownedInteraction.GetButton(M_SELLBUTTON).onClicked.AddListener(() => PlaySound());
        //InteractionButton _unownedInteraction = m_unownedChampInteractionBtnsPrefab.GetComponent<InteractionButton>();
        //_unownedInteraction.GetButton(M_BUYBUTTON).onClicked.AddListener(() => BuyChampion());
        //_unownedInteraction.GetButton(M_BUYBUTTON).onClicked.AddListener(() => PlaySound());
        //Debug.Log("AddListenersToInteractionButtons()");
    }

    private void UpdateZeroOwnedChampionsText()
    {
        if (m_activeDisplay == m_ownedDisplay)
        {
            if (m_activeContainer.IsEmpty())
            {
                m_noOwnedChampionsText.gameObject.SetActive(true);
                if (m_fightersInTraining == null || m_fightersInTraining.Count == 0)
                    m_noOwnedChampionsText.text = M_NOOWNEDCHAMPIONS;
                else
                    m_noOwnedChampionsText.text = M_CHAMPIONSINTRAINING;
            }
            else
                m_noOwnedChampionsText.gameObject.SetActive(false);
        }
    }

    private void SwapFighterContainer(FighterContainer _containerAdd, FighterContainer _containerRemove, FighterData _fighter)
    {
        _containerAdd.Add(_fighter);
        _containerRemove.Remove(_fighter);
        Display(m_activeDisplay.championContent, _containerRemove);
        ClearSelection();
    }

    private void ActiveDisplay(UIFighterDataDisplay _display, FighterContainer _champions)
    {
        if (m_championMenuButton != null)
            DisplayChampionManageButtons(false, new Vector2(), false);
        if (m_activeDisplay == null || m_activeDisplay != _display)
        {
            m_activeContainer = _champions;
            m_activeDisplay?.Disable();
            m_activeDisplay = _display;
            m_activeDisplay.Enable();
            Display(m_activeDisplay.championContent, _champions);
        }
    }

    private void SetSelectedFighter(FighterCard _card = null, string _fighterName = "")
    {
        m_selectedCard = _card;
        m_selectedFighter = m_activeContainer?.Find(_fighterName);
        m_selectedChampion = m_selectedFighter;
    }

    private void LoadChampionData()
    {
        if (m_unownedChampions == null)
        {
            m_unownedChampions = DataHolder.DefaultUnownedChampions();
            //m_unownedChampions = (FighterContainer)
            //    InOut.LoadData(m_unownedChampionData, typeof(FighterContainer));
            //if (m_unownedChampions == null)
            //{
            //    m_unownedChampions = DataHolder.DefaultUnownedChampions();
            //}
        }
        if (m_ownedChampions == null)
        {
            m_ownedChampions = DataHolder.DefaultOwnedChampions();
        }

    }

    private void Display(GameObject _contentHolder, FighterContainer _container)
    {
        bool _ownedContainer = _container == m_ownedChampions;
        ClearDisplay(_contentHolder);
        for (int i = 0; i < _container.Count(); i++)
        {
            FighterCard card = (Instantiate(m_cardPrefab, _contentHolder.transform) as GameObject)
                .GetComponent<FighterCard>();
            card.SetData(_container.Get(i), _ownedContainer);
            card.selectInManagement += SetSelectedFighter;
            card.fighterManagementButtons += DisplayChampionManageButtons;
            //card.OnEnable += ExtraMenuButtons;
            card.onSoundClick += m_audioManager.PlaySlot;
            card.onDisable += ClearSelection;
        }
        UpdateZeroOwnedChampionsText();
    }

    private void ClearDisplay(GameObject _contentHolder)
    {
        Transform[] _cards = _contentHolder.GetComponentsInChildren<Transform>();
        for (int i = 1; i < _cards.Length; i++)
        {
            Destroy(_cards[i].gameObject);
        }
    }

    private void DisplayChampionManageButtons(bool _enable, Vector2 _position)
    {
        DisplayChampionManageButtons(_enable, _position, true);
    }

    private void DisplayChampionManageButtons(bool _enable, Vector2 _position, bool _useAnimation)
    {
        if (_enable)
        {
            Transform _usedHolder = m_championManagementButtonHolders[0];
            int _posIndex = 0;
            int i = 0;
            foreach (Transform _holder in m_championManagementButtonHolders)
            {
                if (_holder.childCount == 0)
                {
                    _usedHolder = _holder;
                    _posIndex = i;
                    break;
                }
                i++;
            }
            _usedHolder.position = _position;
            m_championMenuButton = Instantiate(m_usedInteractionButtons,
                _usedHolder).GetComponent<UIAnimations>();
            m_championManagementButtonPositions[_posIndex] = _usedHolder.position.y - m_activeDisplay.contentHeight;
            AddListenersToInteractionButtons(m_championMenuButton.GetComponent<InteractionButton>(), m_activeDisplay == m_ownedDisplay);
        }
        else
        {
            if (m_selectedCard != null)
            {
                m_selectedCard.OnClickUINull();
                if (_useAnimation)
                    m_championMenuButton.SetAnimation(false);
                else
                    Destroy(m_championMenuButton.gameObject);
            }
        }
    }

    private void UpdateCoins(int _currency)
    {
        m_currency = _currency;
        m_coins.text = $"{_currency} {DataHolder.COIN_ICON}";
    }

    private void UpdateWins()
    {
        m_fightWins.text = $"Fights won: {m_fightsWon}.";
    }

    private void UpdateButtonPositions()
    {
        for (int i = 0; i < m_championManagementButtonHolders.Length; i++)
        {
            if (m_championManagementButtonHolders[i].childCount > 0)
                m_championManagementButtonHolders[i].position = new Vector2(m_championManagementButtonHolders[i].position.x, 
                    m_championManagementButtonPositions[i] + m_activeDisplay.contentHeight);
        }
    }

    protected override void TrainingComplete(KeyValuePair<FighterData, TrainerData> _pair)
    {
        base.TrainingComplete(_pair);
        if (m_activeContainer == m_ownedChampions) {
            Display(m_activeDisplay.championContent, m_activeContainer);
        }
    }

    protected override void Update()
    {
        base.Update();
        UpdateButtonPositions();
    }
}
