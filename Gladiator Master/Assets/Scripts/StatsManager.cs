using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private List<Fighter> m_fighters;
    [SerializeField] private GameObject m_statsTemplatePrefab;
    [SerializeField] private GameObject m_damageTemplatePrefab;

    private Dictionary<Fighter, HealthBar> m_statsUI;
    private float offsetX = 0.8f;
    private float offsetY = 0.7f;
    private float m_damageIndicatorTimer = 1f;

    void Awake()
    {
        m_statsUI = new Dictionary<Fighter, HealthBar>();
        foreach (Fighter _fighter in m_fighters)
        {
            HealthBar _statsTemplate = Instantiate(m_statsTemplatePrefab, this.transform)
                .GetComponent<HealthBar>();
            _statsTemplate.gameObject.SetActive(true);
            _fighter.onDamageUI += (int _damage, int _currentHealth, int _maxHealth) 
                => UpdateUIAfterDamage(_damage, _currentHealth, _maxHealth, _statsTemplate);
            m_statsUI.Add(_fighter, _statsTemplate);
        }
    }

    private void Start()
    {
        foreach (KeyValuePair<Fighter, HealthBar> _pair in m_statsUI)
        {
            int _health = _pair.Key.totalHealth;
            _pair.Value.UpdateHealthBar(_health, _health);
        }
    }

    private void UpdateUIAfterDamage(int _damage, int _currentHealth, int _maxHealth, HealthBar _healthBar)
    {
        PopUpUI _indicator = Instantiate(m_damageTemplatePrefab,
            _healthBar.transform.position, Quaternion.identity, this.transform)
            .GetComponent<PopUpUI>();
        _indicator.MaxTime = m_damageIndicatorTimer;
        _indicator.Text = "-" + _damage;
        _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
    }

    private void UpdateHealthBarPosition()
    {
        foreach (KeyValuePair<Fighter, HealthBar> _pair in m_statsUI)
        {
            Vector3 _positionUI = _pair.Key.transform.position;
            _positionUI += new Vector3(offsetX, offsetY);
            _pair.Value.transform.position = Camera.main.WorldToScreenPoint(_positionUI);
        }
    }

    private void Update()
    {
        UpdateHealthBarPosition();
    }
}
