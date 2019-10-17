using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public TankType m_Type = TankType.Human;

    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;              
    public AudioSource m_ShootingAudio;      
    public AudioClip m_FireClip;     
    public float m_MaxChargeTime = 0.75f;

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

    private void Fire()
    { 
		Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

		shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

		m_ShootingAudio.clip = m_FireClip;
		m_ShootingAudio.Play();
    }
}