using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFatec : MonoBehaviour
{
    public TankAI m_Tank;

    private void Awake()
    {
        m_Tank = GetComponent<TankAI>();
    }

    private void Update()
    {
        if (m_Tank.HasTargetInRange) {
            Vector3 target = m_Tank.Targets[0];
            m_Tank.LookAt(target);
        }
    }
}
