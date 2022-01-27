using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Fighter : MonoBehaviour
{
    public static string attackAnimation = "Attack";
    public static string walkAnimation = "Walking";

    private const string M_PUNCH_ANIM = "Punch";
    private const string M_FALL_ANIM = "Defeated";
    private const string M_ATK_SPEED = "attackSpeed";
    private const string M_WALK_SPEED = "walkSpeed";
    private const int M_ATTACK_AMOUNT = 3;
    private const float M_DEFAULT_DELAY = 1f;
    private const float M_HEALTH_STAMINA_RATIO = 10f;
    private const string M_SHIELD_PREFIX = "shield_";
    private const string M_WEAPON_PREFIX = "weap_";

    [HideInInspector] public FighterData fighterStats;
    public UnityAction<Fighter, bool> onDeathUI;
    public UnityAction<int, int, int> onDamageUI;

    [SerializeField] private Transform m_weaponHolder;
    [SerializeField] private Transform m_shieldHolder;
    [SerializeField] private Transform m_parent;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Fighter m_enemyScript;
    [SerializeField] private AudioManager m_audioManager;
    private Coroutine m_attackCoroutine;
    private float m_targetPositionZ = 1f;
    private float m_movementDirection;
    private float m_movementSpeed = 1f;
    private float m_delayDuration;
    private int m_attackIndex;
    private int m_lastAttackIndex;
    private int m_currentHealth;
    private int m_totalHealth;
    private bool m_movingBeforeCombat = true;
    private bool m_attacking = false;
    private bool m_endOfAnimation = false;

    public int totalHealth
    {
        get
        {
            return (int)(fighterStats.Stamina * M_HEALTH_STAMINA_RATIO - 50); 
        }
    }

    private void Start()
    {
        ManageMovementParameters();
        if (fighterStats != null)
        {
            m_totalHealth = m_currentHealth =
                totalHealth;
            LoadEquipment();
        }
        else
        {
            Debug.LogWarning($"Fighter health not set");
        }
    }

    public void Training(string _animation)
    {
        m_movingBeforeCombat = false;
        if (_animation == attackAnimation)
        {
            Idle();
            float _randomDelay = Random.Range(0f, 1f);
            Coroutine _delayCoroutine = StartCoroutine(AttackModeWithDelay(_randomDelay));
            return;
        }
        m_animator.SetBool(_animation, true);
    }

    public void EndAttackAnimation()
    {
        m_attacking = false;
        m_endOfAnimation = true;
        m_lastAttackIndex = m_attackIndex;
        m_animator.SetInteger(attackAnimation, 0);
    }

    public void AttackSound()
    {
        m_audioManager.PlaySlot(M_PUNCH_ANIM);
    }

    public void DealDamage()
    {
        AttackSound();
        m_enemyScript?.TakeDamage(
            fighterStats.NextDamageValue());
    }

    public void TakeDamage(int _damage)
    {
        float _reductionMultiplier = 1 - Random.Range(0, fighterStats.Agility / 100f);
        if (fighterStats.EquippedShield != null)
            _reductionMultiplier *= fighterStats.EquippedShield.receivedDamageMultiplier;
        int _finalDamage = (int)(_damage * _reductionMultiplier);
        m_currentHealth -= _finalDamage;
        onDamageUI?.Invoke(_finalDamage, m_currentHealth, m_totalHealth);

        if (m_currentHealth <= 0)
        {
            int _deathRate = Random.Range(0, 102);
            bool _fighterAlive = fighterStats.Stamina > _deathRate;
            HandleDefeatAnimation(_fighterAlive);
            onDeathUI?.Invoke(this, _fighterAlive);
        }
    }

    public void StartAttacking()
    {
        Idle();
        AttackSpeedAndDelayDuration();
        m_attackCoroutine = StartCoroutine(AttackMode(m_delayDuration));
    }

    public void StopAttacking()
    {
        if (m_attackCoroutine != null)
        {
            StopCoroutine(m_attackCoroutine);
        }
    }

    public void Idle()
    {
        m_movingBeforeCombat = false;
        m_animator.SetBool(walkAnimation, false);
    }

    private void LoadEquipment()
    {
        if (fighterStats.EquippedWeapon != null && fighterStats.EquippedWeapon.Title != "")
        {
            Instantiate(Resources.Load(M_WEAPON_PREFIX + fighterStats.EquippedWeapon.Title), m_weaponHolder);
        }
        if (fighterStats.EquippedShield != null && fighterStats.EquippedShield.title != "")
        {
            Instantiate(Resources.Load(M_SHIELD_PREFIX + fighterStats.EquippedShield.title), m_shieldHolder);
        }
    }

    private void Move()
    {
        float movementDeltaZ = m_movementDirection * m_movementSpeed * Time.deltaTime;
        m_parent.position = new Vector3(transform.position.x, transform.position.y, 
            transform.position.z + movementDeltaZ);
    }

    private void ManageMovementParameters()
    {
        if (transform.position.z < 0)
        {
            m_targetPositionZ *= -1;
        }
        m_movementDirection = Mathf.Sign(m_targetPositionZ) * -1;
        m_movementSpeed = m_animator.GetFloat(M_WALK_SPEED);
    }

    private void AttackSpeedAndDelayDuration()
    {
        m_animator.SetFloat(M_ATK_SPEED, 1f + fighterStats.Speed * 3f / 100f);
        // delay duration is 0 if fighter speed is 100
        m_delayDuration = M_DEFAULT_DELAY - M_DEFAULT_DELAY * fighterStats.Speed / 100;
        if (m_delayDuration < 0)
        {
            m_delayDuration = 0;
        }
    }

    private IEnumerator AttackMode(float _delay)
    {
        while (true)
        {
            if (m_endOfAnimation)
            {
                m_endOfAnimation = false;
                yield return new WaitForSeconds(_delay);
            }
            else if (!m_attacking)
            {
                do
                {
                    m_attackIndex = (int)Random.Range(1f, M_ATTACK_AMOUNT + 0.99f);
                } while (m_lastAttackIndex == m_attackIndex);
                m_animator.SetInteger(attackAnimation, m_attackIndex);
                m_attacking = true;
            }
            yield return null;
        }
    }

    private IEnumerator AttackModeWithDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        StartAttacking();
    }

    private void HandleDefeatAnimation(bool _alive)
    {
        if (_alive)
        {
            m_animator.SetBool(M_FALL_ANIM, true);
            return;
        }
        m_animator.enabled = false;
    }

    private void Update()
    {
        if (m_movingBeforeCombat)
        {
            Move();
        }
    }
}
