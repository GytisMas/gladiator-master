using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCollider : MonoBehaviour
{
    private int m_fightersInLocation = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Fighter")
        {
            other.GetComponent<Fighter>().StartAttacking();
            m_fightersInLocation++;
            if (m_fightersInLocation >= 2)
            {
                Destroy(gameObject);
            }
        }
    }
}
