using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour, IPoolable
{
    [SerializeField]
    GameObject impactParticles, decalOrigin;

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
        foreach (ParticleSystem p in projectileParticles)
        {
            p.emissionRate = 200;
        }
        direction.y = 0f;
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
        else if (other.GetComponent<IPoolable>() == null && other.GetComponent<Renderer>() != null)
        {
            RaycastHit hit;
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Physics.Raycast(transform.position, hitPoint - transform.position, out hit);
            GameObject crack = GameManager.Instance.CrackPool.GetObject(hitPoint + new Vector3(0f, 1f, 0f), Quaternion.LookRotation((hitPoint + new Vector3(0f, 1f, 0f)) - (decalOrigin.transform.position + new Vector3(0f, 1f, 0f))));
            crack.transform.LookAt(hitPoint + new Vector3(0f, 1f, 0f));
        }
        GameObject particles = Instantiate(impactParticles, transform.position, transform.rotation);
        AudioManager.Instance.SpawnSound("ImpactSound", particles.transform, true, false, false, 0.5f);
        GameManager.Instance.ProjectilePool.RePoolObject(gameObject);
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