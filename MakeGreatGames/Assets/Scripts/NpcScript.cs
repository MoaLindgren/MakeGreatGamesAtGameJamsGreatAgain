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
        generateNewValue = true;
        nearNpc = false;
    }

    private void Start()
    {
        vips = CoinManager.Instance.Coins;
        agent.stoppingDistance = 1f;
        agent.isStopped = false;
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
                if (distance > maxDistance)
                {
                    agent.destination = target.Position;
                }
                else
                {
                    canShoot = true;
                }
            }
            else
            {
                canShoot = false;
                MoveToRandomVIP();
            }
        }
        tower.transform.rotation = Quaternion.Lerp(tower.transform.rotation, Quaternion.LookRotation(target.transform.position - tower.transform.position), towerTurnSpeed * Time.deltaTime);
        if (canShoot && Quaternion.Angle(tower.transform.rotation, Quaternion.LookRotation(target.transform.position - tower.transform.position)) < 2f)
        {
            Shoot();
        }
        if (currentSpecialAttack != Nothing)
        {
            currentSpecialAttack();
        }
        if (coins >= 3)
        {
            StopCoroutine("SpecialAttackTimer");
            StartCoroutine("SpinWheel");
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
        if (Vector3.Distance(transform.position, vips[rnd].transform.position) < agent.stoppingDistance)
        {
            generateNewValue = true;
        }

    }
}