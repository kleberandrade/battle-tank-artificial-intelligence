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
        m_Tank.Move(Random.Range(-1.0f, 1.0f));
        m_Tank.Rotate(Random.Range(-1.0f, 1.0f));
    }
}
