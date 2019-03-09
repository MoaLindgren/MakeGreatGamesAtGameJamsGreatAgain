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
    float speed;

    [SerializeField]
    int damage;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Init(TankScript target, TankScript shooter)
    {
        this.target = target;
        this.shooter = shooter;
        agent.destination = target.transform.position;
    }

    void Update()
    {
        agent.destination = target.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript hitTank = other.GetComponent<TankScript>();
        if (hitTank != null && hitTank != shooter)
        {
            hitTank.TakeDamage(damage);
        }
    }
}