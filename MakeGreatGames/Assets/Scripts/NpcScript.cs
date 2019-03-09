using System.Collections;
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
    bool move, nearNpc;
    PlayerScript target;
    GameObject[] npcs;
    Vector3 npcGoTo;

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
        npcs = GameObject.FindGameObjectsWithTag("Npc");
        nearNpc = false;
    }

    void Update()
    {

        //RAYCAST FÖR ATT HITTA NPC: 

        //foreach(GameObject npc in npcs)
        //{
        //    RaycastHit findNpc;
        //    if (Physics.Raycast(transform.position, (npc.transform.position - transform.position), out findNpc, 50f))
        //    {
        //        Debug.DrawRay(transform.position, (npc.transform.position - transform.position), Color.blue);
        //        if(findNpc.distance < 2f)
        //        {
        //            nearNpc = true;
        //            if(Vector3.Distance(transform.position, target.Position) < Vector3.Distance(npc.transform.position, target.Position))
        //            {
        //                GetComponent<NavMeshObstacle>().enabled = true;
        //            }
        //            else
        //            {
        //                npc.GetComponent<NavMeshObstacle>().enabled = true;
        //            }
        //        }
        //        else
        //        {
        //            nearNpc = false;
        //        }
        //    }
        //}

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

