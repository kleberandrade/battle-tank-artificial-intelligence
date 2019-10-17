using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public TankType m_Type = TankType.Human;

    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public AudioSource m_ShootingAudio;      
    public AudioClip m_FireClip;     
    public float m_MaxChargeTime = 0.75f;

    [SerializeField]
    private float m_RotateSpeed = 180.0f;

    [Header("References")]
    public Transform m_FireTransform;
    public Transform m_TurretTransform;

    private string m_FireButton;        
    [SerializeField]
    private float m_CurrentLaunchForce = 20.0f;

    private bool m_Fired;

    public void StartFiring()
    {
        if (!m_Fired)
        {
            m_Fired = true;
            InvokeRepeating("Fire", 0.0f, m_MaxChargeTime);
        }
    }

    public void StopFiring()
    {
        m_Fired = false;
        CancelInvoke("Fire");
    }

    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
    }

    private void OnEnable()
    {
        if (m_TurretTransform) m_TurretTransform.rotation = transform.rotation;
        StopFiring();
    }

    private void OnDisable()
    {
        StopFiring();
    }

    private void Update()
    {
        if (m_Type == TankType.Human)
        {
            if (Input.GetButtonDown(m_FireButton))
            {
                StartFiring();
            }
            else if (Input.GetButtonUp(m_FireButton))
            {
                StopFiring();
            }
        }
    }

    public void Rotate(float rotate)
    {
        rotate = Mathf.Clamp(rotate, -1.0f, 1.0f);
        m_TurretTransform.Rotate(m_TurretTransform.up, rotate * m_RotateSpeed * Time.deltaTime);
    }

    public void LookAt(Vector3 target)
    {
        target.y = m_TurretTransform.position.y;
        Vector3 direction = target - m_TurretTransform.position;
        float angle = Vector3.SignedAngle(direction, m_TurretTransform.forward, Vector3.up);
        if (Mathf.Abs(angle) > 3.0f)
            Rotate(-angle);
    }

    private void Fire()
    { 
		Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

		shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

		m_ShootingAudio.clip = m_FireClip;
		m_ShootingAudio.Play();
    }

    public void FireAndExplode()
    {
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        ShellExplosion shellExplosion = shellInstance.GetComponent<ShellExplosion>();
        shellExplosion.Explode(15.0f);
    }
}