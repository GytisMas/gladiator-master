using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image m_healthBar;
    [SerializeField] private TextMeshProUGUI m_healthText;

    public void UpdateHealthBar(int _currentHealth, int _maxHealth)
    {
        m_healthBar.fillAmount = (float)_currentHealth / _maxHealth;
        m_healthText.text = _currentHealth + " / " + _maxHealth;
    }
}
