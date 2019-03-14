using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcScript : TankScript, IPoolable
{
    [SerializeField]
    float maxDistance, minDistance;

    [SerializeField]
    int points, rnd;

    bool move, nearNpc, generateNewValue, isActive = false;

    PlayerScript target;

    GameObject[] vips; //very important places, ye

    Vector3 npcGoTo, lastPos;

    NavMeshAgent agent;

    public int Points
    {
        get { return points; }
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
    }

    void Update()
    {
        if (!isActive)
        {
            foreach (ParticleSystem p in frontSmoke)
            {
                p.emissionRate = 0;
            }
            foreach (ParticleSystem p in backSmoke)
            {
                p.emissionRate = 0;
            }
            return;
        }
        if (Vector3.Distance(lastPos, transform.position) < Vector3.Distance(frontSmoke[0].transform.position, transform.position))
        {
            foreach (ParticleSystem p in backSmoke)
            {
                p.emissionRate = 20;
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.emissionRate = 0;
            }
        }
        else if (Vector3.Distance(lastPos, transform.position) > Vector3.Distance(frontSmoke[0].transform.position, transform.position))
        {
            foreach (ParticleSystem p in backSmoke)
            {
                p.emissionRate = 0;
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.emissionRate = 20;
            }
        }
        else
        {
            foreach (ParticleSystem p in backSmoke)
            {
                p.emissionRate = 0;
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.emissionRate = 0;
            }
        }
        bool withinDistance = false;
        float distance = Vector3.Distance(transform.position, target.transform.position);
        RaycastHit hit;
        if (Physics.SphereCast(shotStart.transform.position, 0.1f, (target.transform.position - shotStart.transform.position), out hit))
        {
            Debug.DrawRay(shotStart.transform.position, (target.transform.position - shotStart.transform.position));
            if (hit.transform.tag == "Player")
            {
                agent.stoppingDistance = 4f;
                agent.destination = target.transform.position;
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
        if (coins >= CoinManager.Instance.CoinsToUlt)
        {
            coins -= CoinManager.Instance.CoinsToUlt;
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
        yield return new WaitForSeconds(10);        //uppdatera med variabel
        agent.speed = originalSpeed;
    }

    public void DeActivate()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        agent.enabled = false;
        health = maxHealth;
        coins = 0;
        isActive = false;
    }

    public void Activate()
    {
        healthSlider.value = healthSlider.maxValue;
        isActive = true;
        agent.enabled = true;
        agent.isStopped = false;
    }

    public bool IsActive()
    {
        return isActive;
    }
}