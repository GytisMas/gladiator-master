using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIAnimations : MonoBehaviour
{
    private const string M_ANIM_ACTIVE = "Active";

    [SerializeField] private Animator m_animator;

    private void Awake()
    {
        SetAnimation(true);
    }

    public void SetAnimation(bool _value)
    {
        m_animator.SetBool(M_ANIM_ACTIVE, _value);
    }

    public void EnableObject()
    {
        gameObject.SetActive(true);
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
