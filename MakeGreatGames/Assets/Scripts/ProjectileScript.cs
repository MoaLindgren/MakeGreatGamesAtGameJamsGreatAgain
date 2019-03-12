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
        transform.Translate(direction * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Active)
            return;
        TankScript hitTank = other.gameObject.GetComponent<TankScript>();
        if (hitTank != null && hitTank != shooter)
        {
            hitTank.TakeDamage(damage);
            foreach (ParticleSystem p in projectileParticles)
            {
                p.emissionRate = 0;
            }
            StopCoroutine("DestroyTimer");
            ProjectilePoolScript.Instance.ProjectileDestroyed(gameObject);
        }
        if (hitTank == null || hitTank != shooter)
        {
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