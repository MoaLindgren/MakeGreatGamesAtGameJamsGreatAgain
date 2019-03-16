using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcScript : TankScript, IPoolable
{
    [SerializeField]
    float maxDistance, minDistance;

    [SerializeField]
    int points;

    bool move, isActive = false;

    float distance;

    PlayerScript target;

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
    }

    void Start()
    {
        CoinManager.Instance.CoinSpawned.AddListener(GenerateNewDestination);
    }

    void Update()
    {
        if (!isActive)
        {
            ActivateParticles(backSmoke, false);
            ActivateParticles(frontSmoke, false);
            return;
        }
        ActivateCorrectSmokes();
        distance = Vector3.Distance(transform.position, target.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(shotStart.transform.position, (target.transform.position - shotStart.transform.position), out hit))
        {
            Debug.DrawRay(shotStart.transform.position, (target.transform.position - shotStart.transform.position));
            if (hit.transform.GetComponent<PlayerScript>() != null)
            {
                agent.stoppingDistance = 20f;
                agent.destination = target.transform.position;
                if (distance <= maxDistance)
                {
                    tower.transform.rotation = Quaternion.Lerp(tower.transform.rotation, Quaternion.LookRotation(target.transform.position - tower.transform.position), towerTurnSpeed * Time.deltaTime);
                    if (canShoot && distance <= maxDistance && Quaternion.Angle(tower.transform.rotation, Quaternion.LookRotation(target.transform.position - tower.transform.position)) < 10f)
                    {
                        Shoot();
                    }
                    if (currentSpecialAttack != Nothing)
                    {
                        currentSpecialAttack();
                        currentSpecialAttack = Nothing;
                    }
                }
            }
            else if (Vector3.Distance(transform.position - new Vector3(0f, transform.position.y, 0f), agent.destination - new Vector3(0f, agent.destination.y, 0f)) <= agent.stoppingDistance + 1f)
            {
                GenerateNewDestination();
            }
        }
        if (coins >= CoinManager.Instance.CoinsToUlt)
        {
            coins -= CoinManager.Instance.CoinsToUlt;
            StopCoroutine("SpecialAttackTimer");
            StartCoroutine("SpinWheel");
        }
        lastPos = frontSmoke[0].transform.position;
    }

    void ActivateCorrectSmokes()
    {
        if (Vector3.Distance(lastPos, transform.position) < Vector3.Distance(frontSmoke[0].transform.position, transform.position))
        {
            ActivateParticles(backSmoke, true);
            ActivateParticles(frontSmoke, false);
        }
        else if (Vector3.Distance(lastPos, transform.position) > Vector3.Distance(frontSmoke[0].transform.position, transform.position))
        {
            ActivateParticles(backSmoke, false);
            ActivateParticles(frontSmoke, true);
        }
        else
        {
            ActivateParticles(backSmoke, false);
            ActivateParticles(frontSmoke, false);
        }
    }

    void ActivateParticles(ParticleSystem[] particles, bool activate)
    {
        foreach (ParticleSystem p in particles)
            p.emissionRate = activate ? 20f : 0f;
    }

    public void GenerateNewDestination()
    {
        RaycastHit hit;
        Physics.Raycast(shotStart.position, target.transform.position, out hit);
        if (!isActive || GameManager.Instance.Paused || hit.transform.CompareTag("Player"))
            return;
        agent.stoppingDistance = 0f;
        int coinIndex = Random.Range(0, CoinManager.Instance.Coins.Length);
        float coinDistance = Mathf.Infinity;
        for (int i = 0; i < CoinManager.Instance.Coins.Length; i++)
        {
            if(CoinManager.Instance.Coins[i].activeSelf && Vector3.Distance(transform.position, CoinManager.Instance.Coins[i].transform.position) < coinDistance)
            {
                coinDistance = Vector3.Distance(transform.position, CoinManager.Instance.Coins[i].transform.position);
                coinIndex = i;
            }
        }
        agent.destination = CoinManager.Instance.Coins[coinIndex].transform.position;
    }

    protected override IEnumerator SpeedBoosted()
    {
        float originalSpeed = agent.speed;
        agent.speed *= 2;
        float originalAngular = agent.angularSpeed;
        agent.angularSpeed *= 2;
        yield return new WaitForSeconds(10);        //uppdatera med variabel
        agent.speed = originalSpeed;
        agent.angularSpeed = originalAngular;
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