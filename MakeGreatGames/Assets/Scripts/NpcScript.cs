using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcScript : TankScript
{
    NavMeshAgent agent;
    [SerializeField]
    float maxDistance;
    [SerializeField]
    bool move;
    PlayerScript target;

    public void SetAlive(bool alive)
    {
        this.alive = alive;
    }

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<PlayerScript>();
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, target.Position) > maxDistance)
        {
            agent.isStopped = false;
            agent.destination = target.Position;
        }
        else
        {
            agent.isStopped = true;
        }
    }

}
