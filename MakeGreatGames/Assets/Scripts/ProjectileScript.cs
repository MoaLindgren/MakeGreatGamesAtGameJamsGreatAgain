using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void ShootMe();
}

public class ProjectileScript : MonoBehaviour, IProjectile
{
    [SerializeField]
    GameObject impactParticles;

    [SerializeField]
    MeshRenderer projectileMesh;

    [SerializeField]
    ParticleSystem[] projectileParticles;

    Vector3 direction = Vector3.zero;

    TankScript shooter;

    float speed = 0.0f;

    int damage;

    bool active;

    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    void Awake()
    {
        foreach (ParticleSystem p in projectileParticles)
        {
            p.emissionRate = 0;
        }
        //Render(false);
    }

    public void Init(Vector3 direction, float speed, TankScript parent, int damage)
    {
        foreach (ParticleSystem p in projectileParticles)
        {
            p.emissionRate = 200;
        }
        this.direction = direction;
        this.speed = speed;
        this.shooter = parent;
        this.damage = damage;
        StartCoroutine("DestroyTimer");
    }

    void Update()
    {
        if (!Active || GameManager.Instance.Paused)
            return;
        transform.Translate(direction * Time.deltaTime * speed, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Active)
            return;
        TankScript hitTank = other.gameObject.GetComponent<TankScript>();
        if (hitTank != null && hitTank != shooter)
        {
            Instantiate(impactParticles, transform.position, transform.rotation);
            hitTank.TakeDamage(damage);
            foreach (ParticleSystem p in projectileParticles)
            {
                p.emissionRate = 0;
            }
            StopCoroutine("DestroyTimer");
            ProjectilePoolScript.Instance.ProjectileDestroyed(gameObject);
        }
        else if (hitTank == null)
        {
            Instantiate(impactParticles, transform.position, transform.rotation);
            foreach (ParticleSystem p in projectileParticles)
            {
                p.emissionRate = 0;
            }
            StopCoroutine("DestroyTimer");
            ProjectilePoolScript.Instance.ProjectileDestroyed(gameObject);
        }
    }

    public void Render(bool render)
    {
        projectileMesh.enabled = render;
    }

    public void ShootMe()
    {
        return;
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(15);
        foreach (ParticleSystem p in projectileParticles)
        {
            p.emissionRate = 0;
        }
        ProjectilePoolScript.Instance.ProjectileDestroyed(gameObject);
    }
}