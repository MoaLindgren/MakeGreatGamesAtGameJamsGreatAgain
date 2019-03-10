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

        agent.isStopped = false;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.Position);
        RaycastHit hit;
        if (Physics.SphereCast(shotStart.transform.position, 0.1f, (target.Position - shotStart.transform.position), out hit))
        {
            Debug.DrawRay(shotStart.transform.position, (target.Position - shotStart.transform.position));
            print(hit.transform.gameObject);
            if (hit.transform.tag == "Player")
            {
                agent.stoppingDistance = 4f;
                agent.destination = target.Position;
                if (distance < maxDistance)
                {
                    tower.transform.rotation = Quaternion.Lerp(tower.transform.rotation, Quaternion.LookRotation(target.transform.position - tower.transform.position), towerTurnSpeed * Time.deltaTime);
                    canShoot = true;
                }
            }
            else
            {
                canShoot = false;
                
                MoveToRandomVIP();
            }
        }
        
        if (canShoot && Quaternion.Angle(tower.transform.rotation, Quaternion.LookRotation(target.transform.position - tower.transform.position)) < 10f)
        {
            Shoot();
        }
        if (currentSpecialAttack != Nothing)
        {
            currentSpecialAttack();
            currentSpecialAttack = Nothing;
        }
        if (coins >= 3)
        {
            coins -= 3;
            StopCoroutine("SpecialAttackTimer");
            StartCoroutine("SpinWheel");
        }
    }

    void MoveToRandomVIP()
    {
        agent.stoppingDistance = 1f;
        if (generateNewValue)
        {
            rnd = Random.Range(0, vips.Length);
            generateNewValue = false;
        }

        agent.destination = vips[rnd].transform.position;

        if (Vector3.Distance(transform.position, vips[rnd].transform.position) < agent.stoppingDistance)
        {
            generateNewValue = true;
        }
    }

    protected override IEnumerator SpeedBoosted()
    {
        float originalSpeed = agent.speed;
        agent.speed *= 2;
        yield return new WaitForSeconds(10);
        agent.speed = originalSpeed;
    }
}