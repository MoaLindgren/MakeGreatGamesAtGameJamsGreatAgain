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
    int rnd;
    bool move, nearNpc, generateNewValue;
    PlayerScript target;
    GameObject[] vips; //very important places, ye
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
        vips = CoinManager.Instance.Coins;
        generateNewValue = false;
        nearNpc = false;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.Position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (target.Position - transform.position), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, (target.Position - transform.position));
            if (hit.transform.tag == "Player")
            {
                //print("npc can see the player");
                agent.destination = target.Position;
                move = true;
                if (distance > maxDistance)
                {
                    if (distance < minDistance)
                    {
                        //print("npc can see the player and is within range to shoot");
                        move = false;
                    }
                }
            }
            else
            {
                //print("i cant see the player");
                move = true;
                MoveToRandomVIP();
            }
        }

        if (move)
        {
            agent.isStopped = false;

        }
        else
        {
            agent.isStopped = true;
        }

    }
    void MoveToRandomVIP()
    {

        if (generateNewValue)
        {
            rnd = Random.Range(0, vips.Length);
            generateNewValue = false;
        }

        agent.destination = vips[rnd].transform.position;
        print(Vector3.Distance(transform.position, vips[rnd].transform.position));
        if (Vector3.Distance(transform.position, vips[rnd].transform.position) < 1)
        {
            generateNewValue = true;
        }

    }
}