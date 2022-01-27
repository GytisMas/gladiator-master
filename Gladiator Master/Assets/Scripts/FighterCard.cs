using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FighterCard : MonoBehaviour
{
    public UnityAction<bool, Vector2> fighterManagementButtons;
    public UnityAction onDisable;
    public UnityAction<string> onSoundClick;
    public UnityAction<FighterCard, string> selectInManagement;

    private static GameObject m_usedRenderTexture;
    private static bool m_activeButtonExists = false;
    //private static UIAnimations m_usedButton;

    private string m_fighterName;
    [SerializeField] private GameObject m_renderUI;
    private bool m_isActiveButton = false;
    //private UIAnimations m_buttonsUI;
    [Space]
    [Header("Champion Data")]
    [SerializeField] private TextMeshProUGUI m_price;
    [SerializeField] private TextMeshProUGUI m_name;
    [SerializeField] private TextMeshProUGUI m_strength;
    [SerializeField] private TextMeshProUGUI m_speed;
    [SerializeField] private TextMeshProUGUI m_agility;
    [SerializeField] private TextMeshProUGUI m_stamina;
    [SerializeField] private TextMeshProUGUI m_weapon;
    [SerializeField] private TextMeshProUGUI m_shield;

    public string FIGHTERNAME
    {
        get
        {
            return m_fighterName;
        }
    }

    private void Start()
    {
        m_activeButtonExists = false;
        m_isActiveButton = false;
        m_renderUI.SetActive(false);
    }

    public void Highlightbutton()
    {
        if (m_activeButtonExists)
        {
            if (m_isActiveButton)
            {
                onDisable?.Invoke();
                m_activeButtonExists = false;
                m_isActiveButton = false;
                //Debug.Log("true/true path");
                return;
            }
            //Debug.Log("true/false path");
            m_usedRenderTexture.SetActive(false);
            fighterManagementButtons?.Invoke(false, new Vector2());
        }
        selectInManagement?.Invoke(this, m_fighterName);
        fighterManagementButtons?.Invoke(true, transform.position);
        //m_buttonsUI = Instantiate(m_managementButtons, transform).GetComponent<UIAnimations>();
        //m_usedButton = m_buttonsUI;
        m_activeButtonExists = true;
        m_isActiveButton = true;
        m_usedRenderTexture = m_renderUI;
        m_usedRenderTexture.SetActive(true);
        onSoundClick?.Invoke(AudioManager.CARD_CLICK);
    }

    public void OnClickUINull()
    {
        if (m_usedRenderTexture != null)
            m_usedRenderTexture.SetActive(false);
        m_usedRenderTexture = null;
        m_isActiveButton = false;
        m_activeButtonExists = false;
    }

    public void SetData(FighterData _fighter, bool _owned = false)
    {
        int _displayedPrice = _owned ? (int)(_fighter.Price * DataHolder.SELL_RATIO) : _fighter.Price;
        m_fighterName = _fighter.Name;
        m_price.text = $"Price: {_displayedPrice} {DataHolder.COIN_ICON}";
        m_name.text = _fighter.Name;
        m_strength.text = "Strength: " + _fighter.Strength;
        m_speed.text = "Speed: " + _fighter.Speed;
        m_agility.text = "Agility: " + _fighter.Agility;
        m_stamina.text = "Stamina: " + _fighter.Stamina;
        m_weapon.text = "Weapon: ";
        m_shield.text = "Shield: ";
        m_weapon.text += _fighter.EquippedWeapon != null && _fighter.EquippedWeapon.Title != "" ? _fighter.EquippedWeapon.Title : "none";
        m_shield.text += _fighter.EquippedShield != null && _fighter.EquippedShield.title != "" ? _fighter.EquippedShield.title : "none";
    }
}
