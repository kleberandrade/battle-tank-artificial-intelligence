using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(TankMovement))]
[RequireComponent(typeof(TankShooting))] 
[RequireComponent(typeof(TankHealth))]
[RequireComponent(typeof(TankRadar))]
public class TankAI : MonoBehaviour
{
    #region [ Fields ]
    private TankMovement m_MovementScript;
    private TankShooting m_ShootingScript;
    private TankHealth m_HealthScript;
    private TankRadar m_RadarScript;
    private NavMeshAgent m_Agent;
    #endregion

    #region [ Unity Methods ]
    private void Awake()
    {
        m_MovementScript = GetComponent<TankMovement>();
        m_ShootingScript = GetComponent<TankShooting>();
        m_HealthScript = GetComponent<TankHealth>();
        m_RadarScript = GetComponent<TankRadar>();
        m_Agent = GetComponent<NavMeshAgent>();
    }
    #endregion

    #region [ Targets Methods ]
    public List<Vector3> Targets => m_RadarScript.m_Targets;

    public bool HasTargetInRange => Targets.Count > 0;
    #endregion

    #region [ Turret Methods ]
    public Vector3 TurretDirection => m_ShootingScript.m_FireTransform.forward;

    public void TurretRotate(float rotate)
    {
        m_ShootingScript.Rotate(rotate);
    }

    public void TurretLookAt(Vector3 target)
    {
        m_ShootingScript.LookAt(target);
    }
    #endregion

    #region [ Manual Movement ]
    public Vector3 Position => m_MovementScript.Position;

    public void Move(float movement)
    {
        m_MovementScript.Move(movement);
    }

    public void Rotate(float rotate)
    {
        m_MovementScript.Turn(rotate);
    }
    public void LookAt(Vector3 target)
    {
        float angle = Angle(target);
        if (Mathf.Abs(angle) > 3.0f)
        {
            Debug.Log(angle);
            Rotate(-angle);
        }
    }
    #endregion

    #region [ NavMesh Movement ]
    public NavMeshAgent Agent => m_Agent;

    public bool SetDestination(Vector3 target)
    {
        return m_Agent.SetDestination(target);
    }

    public void StopMotionToDestination()
    {
        m_Agent.isStopped = true;
    }
    #endregion

    #region [ Fire Methods ]
    public void StartFire()
    {
        m_ShootingScript.StartFiring();
    }

    public void StopFire()
    {
        m_ShootingScript.StopFiring();
    }
    #endregion

    #region [ Health Methods ]
    /// <summary>
    /// Life of your tank
    /// </summary>
    public float Health => m_HealthScript.Health;

    /// <summary>
    /// Self destruction
    /// </summary>
    public void SelfDestruction()
    {
        m_HealthScript.SelfDestruction();
    }
    #endregion

    #region [ Helpers Methods ]
    /// <summary>
    /// Distance between your tank and another position
    /// </summary>
    /// <param name="target">Target distance</param>
    /// <returns></returns>
    public float DistanceToTarget(Vector3 target)
    {
        return Vector3.Distance(target, Position);
    }

    /// <summary>
    /// Direction between your tank and another position
    /// </summary>
    /// <param name="target">Target direction</param>
    /// <returns></returns>
    public Vector3 Direction(Vector3 target)
    {
        return target - transform.position;
    }

    /// <summary>
    /// Angle between your tank and another position
    /// </summary>
    /// <param name="target">Target position</param>
    /// <returns></returns>
    public float Angle(Vector3 target)
    {
        Vector3 targetDir = Direction(target);
        return Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
    }
    #endregion
}
