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
    Vector3 lastPos;

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
        lastPos = frontSmoke[0].transform.position;
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
        if (Vector3.Distance(lastPos, transform.position) < Vector3.Distance(frontSmoke[0].transform.position, transform.position))
        {
            foreach(ParticleSystem p in backSmoke)
            {
                p.Play();
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.Stop();
            }
        }
        else if (Vector3.Distance(lastPos, transform.position) > Vector3.Distance(frontSmoke[0].transform.position, transform.position))
        {
            foreach (ParticleSystem p in backSmoke)
            {
                p.Stop();
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.Play();
            }
        }
        else
        {
            foreach (ParticleSystem p in backSmoke)
            {
                p.Stop();
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.Stop();
            }
        }
        bool withinDistance = false;
        float distance = Vector3.Distance(transform.position, target.Position);
        RaycastHit hit;
        if (Physics.SphereCast(shotStart.transform.position, 0.1f, (target.Position - shotStart.transform.position), out hit))
        {
            Debug.DrawRay(shotStart.transform.position, (target.Position - shotStart.transform.position));
            if (hit.transform.tag == "Player")
            {
                agent.stoppingDistance = 4f;
                agent.destination = target.Position;
                if (distance < maxDistance)
                {
                    tower.transform.rotation = Quaternion.Lerp(tower.transform.rotation, Quaternion.LookRotation(target.transform.position - tower.transform.position), towerTurnSpeed * Time.deltaTime);
                    withinDistance = true;
                }
            }
            else
            {
                MoveToRandomVIP();
            }
        }

        if (canShoot && withinDistance && Quaternion.Angle(tower.transform.rotation, Quaternion.LookRotation(target.transform.position - tower.transform.position)) < 10f)
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
        lastPos = frontSmoke[0].transform.position;
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