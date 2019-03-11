using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class MissileScript : MonoBehaviour, IProjectile
{
    [SerializeField]
    int damage;
    
    [SerializeField]
    GameObject blast;

    TankScript target, shooter;

    NavMeshAgent agent;

    Material mat;

    float sinPos = 0f, colorAmount = 127.5f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent.destination == null)
        {
            agent.destination = CoinManager.Instance.Coins[Random.Range(0, CoinManager.Instance.Coins.Length)].transform.position;
        }
        mat = GetComponentInChildren<MeshRenderer>().materials[0];
    }

    public void Init(TankScript target, TankScript shooter)
    {
        this.target = target;
        this.shooter = shooter;
        agent.destination = target.transform.position;
    }

    void Update()
    {
        if (GameManager.Instance.Paused)
            return;
        sinPos += Time.deltaTime;
        colorAmount = 0.5f + (Mathf.Lerp(-1, 1, Mathf.Sin(sinPos)) / 2);
        mat.SetColor("_EmissionColor", new Color(colorAmount, colorAmount, colorAmount));
        if (colorAmount == 0f)
        {
            sinPos = 0f;
        }
        TankScript[] enemies = FindObjectsOfType<NpcScript>();
        if (target == null && enemies.Length > 0)
        {
            target = enemies[Random.Range(0, enemies.Length)];
        }
        if (target != null)
            agent.destination = target.transform.position;
        else if (Vector3.Distance(transform.position, agent.destination) < 0.1f)
        {
            Instantiate(blast, transform.position, Quaternion.identity);
        }
    }

    public void ShootMe()
    {
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript hitTank = other.GetComponent<TankScript>();
        IProjectile hitProjectile = other.GetComponent<IProjectile>();
        if (hitTank != null && hitTank != shooter)
        {
            hitTank.TakeDamage(damage);
            Instantiate(blast, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (hitProjectile != null)
        {
            hitProjectile.ShootMe();
        }
    }
}