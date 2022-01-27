using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FightManager : GameManager
{
    private const float m_priceToWinRatio = 0.3f;
    private const float m_priceToResurrectionRatio = 0.6f;
    private const string m_fightWonText = "You have won. For winning this fight, you will gain ";
    private const string m_fightLostText = "You have lost.";
    private const string m_fighterAlive = " Your fighter survived the fight.";
    private const string m_fighterDead = " Your fighter has died during the fight. " +
        "To resurrect him, you will need to pay a fee of ";
    private const string m_feePaidText = "You have paid the resurrection fee. You can return to the menu.";
    private const string m_feeNotPaidText = "You don't have sufficient funds to pay the fee. You can return to the menu.";

    private readonly string[] m_weapons = { "sword", "mace", "club" };
    private readonly string[] m_shields = { "hex", "square", "round" };

    private static bool m_fightWon = false;
    private static bool m_feePaid = true;

    private int[] m_statBoostInterval = new int[] { 1, 15 };
    [SerializeField] private GameObject m_damageIndicatorPrefab;
    [SerializeField] private Fighter m_playerFighter;
    [SerializeField] private Fighter m_enemyFighter;
    [SerializeField] private GameObject m_fightOverUI;
    [SerializeField] private GameObject m_payFeeButtonUI;
    [SerializeField] private TextMeshProUGUI m_fightOverText;

    private int m_resurrectionFee
    {
        get
        {
            return (int)(m_playerFighter.fighterStats.Price * m_priceToResurrectionRatio);
        }
    }

    private int m_winPayment
    {
        get
        {
            return (int)(m_enemyFighter.fighterStats.Price * m_priceToWinRatio);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        SetPlayerData();
        DefaultEnemyData();
        m_playerFighter.onDeathUI = m_enemyFighter.onDeathUI = AfterFightUI;
    }

    private void Start()
    {
        m_payFeeButtonUI.SetActive(false);
        m_fightOverUI.SetActive(false);
    }

    public void PayResurrectionFee()
    {
        if (m_currency >= m_resurrectionFee)
        {
            m_feePaid = true;
            m_currency -= m_resurrectionFee;
            m_fightOverText.text = m_feePaidText;
        }
        else
        {
            m_fightOverText.text = m_feeNotPaidText;
        }
        m_payFeeButtonUI.SetActive(false);
    }

    public void GoToManagementScene()
    {
        if (!m_fightWon && !m_feePaid)
        {
            m_ownedChampions.Remove(m_selectedFighter);
            m_selectedFighter = null;
        }
        ManagementScene();
    }

    private void AfterFightUI(Fighter _fighter, bool _alive)
    {
        m_fightWon = _fighter == m_enemyFighter;
        m_fightOverUI.SetActive(true);
        m_fightOverText.text = !m_fightWon ? m_fightLostText : 
            m_fightWonText + $"{m_winPayment} {DataHolder.COIN_ICON}";
        if (!m_fightWon)
        {
            m_fightOverText.text += _alive ? m_fighterAlive : 
                m_fighterDead + $"{m_resurrectionFee} {DataHolder.COIN_ICON}";
            if (!_alive)
            {
                m_payFeeButtonUI.SetActive(true);
                m_feePaid = false;
            }
        }
        else
        {
            m_currency += m_winPayment;
            m_fightsWon++;
            m_playerFighter.fighterStats.RandomBoostAllAttributes();
        }
        if (_fighter == m_playerFighter)
        {
            m_enemyFighter.StopAttacking();
        }
        else
        {
            m_playerFighter.StopAttacking();
        }
    }

    private void SetPlayerData()
    {
        FighterData _fighter = m_selectedFighter;
        if (_fighter == null)
        {
            m_playerFighter.fighterStats = new FighterData("PlayerName", 80, 45, 20, 50);
            return;
        }
        m_playerFighter.fighterStats = _fighter;
    }

    private void DefaultEnemyData()
    {
        //m_enemyFighter.FighterStats = new FighterData("EnemyName", 10, 10, 10, 10000);
        m_enemyFighter.fighterStats.Strength = SetAttribute(m_fightsWon);
        m_enemyFighter.fighterStats.Speed = SetAttribute(m_fightsWon);
        m_enemyFighter.fighterStats.Agility = SetAttribute(m_fightsWon);
        m_enemyFighter.fighterStats.Stamina = SetAttribute(m_fightsWon);
        m_enemyFighter.fighterStats.EquippedWeapon = new Weapon(m_weapons[Random.Range(0, m_shields.Length)]);
        m_enemyFighter.fighterStats.EquippedShield = new Shield(m_shields[Random.Range(0, m_shields.Length)]);
    }
    private int SetAttribute(int _fightsWon)
    {
        if (_fightsWon > 18)
        {
            _fightsWon = 18;
        }
        int _attribute = 10 + _fightsWon * 7;
        _attribute = (int)Random.Range(_attribute * 0.8f, _attribute * 1.2f);
        return Mathf.Clamp(_attribute, 10, 100);
    }
}
