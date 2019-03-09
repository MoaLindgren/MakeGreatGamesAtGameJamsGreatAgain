using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MissileScript : MonoBehaviour
{
    TankScript target, shooter;

    NavMeshAgent agent;

    [SerializeField]
    int damage;

    [SerializeField]
    GameObject blast;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent.destination == null)
        {
            agent.destination = CoinManager.Instance.Coins[Random.Range(0, CoinManager.Instance.Coins.Length)].transform.position;
        }
    }

    public void Init(TankScript target, TankScript shooter)
    {
        this.target = target;
        this.shooter = shooter;
        agent.destination = target.transform.position;
    }

    void Update()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        TankScript hitTank = other.GetComponent<TankScript>();
        if (hitTank != null && hitTank != shooter)
        {
            hitTank.TakeDamage(damage);
            Instantiate(blast, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}