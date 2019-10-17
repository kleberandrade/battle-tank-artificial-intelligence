using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              

    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }

    public void Explode(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidBody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidBody)
            {
                continue;
            }

            targetRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, radius);

            TankHealth targetHealth = targetRigidBody.GetComponent<TankHealth>();

            if (!targetHealth)
            {
                continue;
            }

            float damage = CalculateDamage(targetRigidBody.position);

            targetHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject);
    }

    public void Explode()
    {
        Explode(m_ExplosionRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
		Vector3 explosionToTarget = targetPosition - transform.position;
		float explosionDistance = explosionToTarget.magnitude;
		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
		float damage = relativeDistance * m_MaxDamage;
		damage = Mathf.Max(0f, damage);
		return damage;
    }
}