using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcScript : TankScript
{
    NavMeshAgent agent;
    float maxDistance;
    [SerializeField]
    float speed;
    [SerializeField]
    bool move;
    PlayerScript target;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<PlayerScript>();
    }

    void Update()
    {
        if(move)
        {
            agent.Move(target.Position);
        }
    }

}
