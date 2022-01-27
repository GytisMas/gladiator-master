using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIFighterDataDisplay : MonoBehaviour
{
    public GameObject championContent;
    [SerializeField] private GameObject m_scrollView;
    //[SerializeField] private UIAnimations m_buttonsOnClick;
    //[SerializeField] private GameObject m_buttonsNonClick;

    public float contentHeight
    {
        get
        {
            return championContent.transform.position.y;
        }
    }


    private void Awake()
    {
        Disable();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        m_scrollView.SetActive(true);
        //m_buttonsNonClick.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        m_scrollView.SetActive(false);
        //m_buttonsNonClick.SetActive(false);
        //m_buttonsOnClick.gameObject.SetActive(false);
    }

    //public void DisableOnClick()
    //{
    //    m_buttonsOnClick.SetActive(false);
    //}

    //public void SetOnClick(bool _value, Vector3 _pos)
    //{
    //    m_buttonsOnClick.gameObject.SetActive(_value);
    //    m_buttonsOnClick.transform.parent.position = _pos;
    //    Debug.Log(_pos);
    //    if (_value) {
    //        m_buttonsOnClick.SetActive(true);
    //    }
    //}

    //private void Update()
    //{
    //    Debug.Log(m_buttonsOnClick.transform.parent.position);
    //}
}
