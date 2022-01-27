using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLayout3D : MonoBehaviour
{
    [SerializeField] private float m_spacing = 1f;
    [SerializeField] private float m_maxColumns = 1f;
    private List<Transform> m_elements;


    void Start()
    {

    }

    private void GetLayoutElements()
    {
        Transform[] _allTransforms = gameObject.GetComponentsInChildren<Transform>();
        m_elements = new List<Transform>();
        for (int i = 1; i <= _allTransforms.Length - 1; i += 44)
        {
            m_elements.Add(_allTransforms[i]);
        }
    }

    private void SetLayout()
    {
        GetLayoutElements();
        int _currentColumn = 0;
        for (int i = 0; i < m_elements.Count; i++)
        {
            Vector3 pos = transform.position;
            float deltaX = m_spacing * (i % m_maxColumns);
            //Debug.Log($"{i} % {m_maxColumns} = {deltaX}");
            if (i % m_maxColumns == 0)
            {
                _currentColumn++;
            }
            pos.z -= _currentColumn * m_spacing;
            pos.x += deltaX;
            m_elements[i].position = pos;
        }
    }

    private void Update()
    {
        SetLayout();
    }
}
