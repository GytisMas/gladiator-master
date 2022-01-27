using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PulseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent {}

    public bool active = true;
    public float maxScale = 1.1f;
    public float scaleTime = 0.5f;
    public ButtonEvent onClicked;

    private float m_scaleTarget;
    private bool m_hasAudioController;
    private AudioController m_audioController;
    
    // Start is called before the first frame update
    virtual internal void Awake()
    {
        m_scaleTarget = scaleAverage;
        m_hasAudioController = (m_audioController = gameObject.GetComponent<AudioController>()) != null;

        if (onClicked == null) {
            onClicked = new ButtonEvent();
        }
    }

    public virtual void OnPointerDown(PointerEventData _eventData)
    {
        if (!active) {
            return;
        }
        
        m_scaleTarget = maxScale;
        if (m_hasAudioController) {
            m_audioController.PlaySlot("down");
        }
    }
    
    public virtual void OnPointerClick(PointerEventData _eventData)
    {
        if (!active) {
            return;
        }
        
        onClicked?.Invoke();
        if (m_hasAudioController) {
            m_audioController.PlaySlot("click");
        }
    }    

    public virtual void OnPointerUp(PointerEventData _eventData)
    {
        if (!active) {
            return;
        }        
        
        m_scaleTarget = 1f;
        if (m_hasAudioController) {
            m_audioController.PlaySlot("up");
        }
    }
    
    public void ImitateClick()
    {
        ImitateClick(false);
    }

    public void ImitateClick(bool _checkActive)
    {
        if (_checkActive && !gameObject.activeSelf) {
            return;
        }
        OnPointerClick(null);
    }

    private float scaleAverage
    {
        get { return (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f; }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_scaleTarget != scaleAverage) {
            Vector3 _newScale = transform.localScale;
            float _timeDelta = Time.deltaTime / scaleTime;
            
            if (scaleAverage < m_scaleTarget) {
                _newScale.x += _timeDelta;
                _newScale.y += _timeDelta;
                _newScale.z += _timeDelta;

                if (_newScale.x > m_scaleTarget || _newScale.y > m_scaleTarget || _newScale.z > m_scaleTarget) {
                    _newScale.x = _newScale.y = _newScale.z = m_scaleTarget;           
                }
            } else {
                _newScale.x -= _timeDelta;
                _newScale.y -= _timeDelta;
                _newScale.z -= _timeDelta;

                if (_newScale.x < m_scaleTarget || _newScale.y < m_scaleTarget || _newScale.z < m_scaleTarget) {
                    _newScale.x = _newScale.y = _newScale.z = m_scaleTarget;           
                }                
            }

            transform.localScale = _newScale;
        }
    }
}
