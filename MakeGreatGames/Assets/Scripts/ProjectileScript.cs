using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour, IPoolable
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

    void Awake()
    {
        foreach (ParticleSystem p in projectileParticles)
        {
            p.emissionRate = 0;
        }
    }

    public void Init(Vector3 direction, float speed, TankScript parent, int damage)
    {
        active = true;
        Render(true);
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
        if (!active || GameManager.Instance.Paused)
            return;
        transform.Translate(direction * Time.deltaTime * speed, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
            return;
        TankScript hitTank = other.gameObject.GetComponent<TankScript>();
        if (hitTank != null && hitTank == shooter)
        {
            return;
        }
        else if (hitTank != null)
        {
            hitTank.TakeDamage(damage);
        }
        Instantiate(impactParticles, transform.position, transform.rotation);
        GameManager.Instance.ProjectilePool.RePoolObject(gameObject);
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
        GameManager.Instance.ProjectilePool.RePoolObject(gameObject);
    }

    public void DeActivate()
    {
        StopCoroutine("DestroyTimer");
        Render(false);
        active = false;
        foreach (ParticleSystem p in projectileParticles)
        {
            p.emissionRate = 0;
        }
    }

    public void Activate()
    {
        return;
    }

    public bool IsActive()
    {
        return active;
    }
}