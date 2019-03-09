﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcScript : TankScript
{
    NavMeshAgent agent;
    [SerializeField]
    float maxDistance, minDistance;
    [SerializeField]
    int points;
    [SerializeField]
    bool move, findDestianation;
    PlayerScript target;

    public int Points
    {
        get { return points; }
    }

    public void SetAlive(bool alive)
    {
        this.alive = alive;
    }

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<PlayerScript>();
        findDestianation = true;
    }

    void Update()
    {
        if (findDestianation)
        {
            float distance = Vector3.Distance(transform.position, target.Position);
            if (distance > maxDistance)
            {
                move = true;
                if (distance < minDistance)
                {
                    print("npc within shooting range");

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, (target.Position - transform.position), out hit, 50f))
                    {
                        Debug.DrawRay(transform.position, (target.Position - transform.position));
                        if (hit.transform.tag == "Player")
                        {
                            print("npc have a clear shot");
                            move = false;
                        }

                    }
                }
            }
            else
            {
                move = false;
            }

            if (move)
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

    void OnTriggerEnter(Collider other)
    {
        //if(other.tag == "Npc")
        //{
        //    print("Uh");
        //    other.GetComponent<NavMeshObstacle>().enabled = true;
        //}
    }

}

