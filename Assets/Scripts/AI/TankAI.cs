using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankAI : MonoBehaviour
{
    private TankMovement m_MovementScript;
    private TankShooting m_ShootingScript;
    private TankHealth m_HealthScript;
    private TankRadar m_RadarScript;
    private NavMeshAgent m_Agent;

    public float Health => m_HealthScript.Health;

    public Vector3 Position => m_MovementScript.Position;

    public List<Vector3> Targets => m_RadarScript.m_Targets;

    public bool HasTargetInRange => Targets.Count > 0;

    public NavMeshAgent Agent => m_Agent;

    public float DistanceToTarget(Vector3 targetPosition)
    {
        return Vector3.Distance(targetPosition, Position);
    }
   
    private void Awake()
    {
        m_MovementScript = GetComponent<TankMovement>();
        m_ShootingScript = GetComponent<TankShooting>();
        m_HealthScript = GetComponent<TankHealth>();
        m_RadarScript = GetComponent<TankRadar>();
        m_Agent = GetComponent<NavMeshAgent>();
    }

    public void Move(float movement)
    {
        m_MovementScript.Move(movement);
    }

    public void Rotate(float rotate)
    {
        m_MovementScript.Turn(rotate);
    }

    public Vector3 Direction(Vector3 target)
    {
        return target - transform.position; ;
    }

    public float Angle(Vector3 target)
    {
        Vector3 targetDir = Direction(target);
        return Vector3.Angle(targetDir, transform.forward);
    }

    public void LookAt(Vector3 target)
    {
        float angle = Angle(target);
        Rotate(angle);
    }

    public void SelfDestruction()
    {
        m_HealthScript.TakeDamage(100.0f);    
    }

    public void StartFire()
    {
        m_ShootingScript.StartFiring();
    }

    public void StopFire()
    {
        m_ShootingScript.StopFiring();
    }
}
