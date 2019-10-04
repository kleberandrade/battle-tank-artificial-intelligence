using System.Collections.Generic;
using UnityEngine;

public class TankRadar : MonoBehaviour
{
    [SerializeField]
    private float m_Radius = 20.0f;

    [SerializeField]
    private LayerMask m_CullingMask;

    public readonly List<Vector3> m_Targets = new List<Vector3>();

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_Radius, m_CullingMask);

        m_Targets.Clear();
        foreach (Collider collider in colliders)
        {
            Vector3 position = collider.transform.position;
            if (transform.position != position)
                m_Targets.Add(position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.15f);
        Gizmos.DrawSphere(transform.position, m_Radius);
    }
}
