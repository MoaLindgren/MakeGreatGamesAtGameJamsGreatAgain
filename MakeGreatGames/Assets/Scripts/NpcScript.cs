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

    bool isActive = false;

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
    }

    protected override void Start()
    {
        base.Start();
        CoinManager.Instance.CoinSpawned.AddListener(GenerateNewDestination);
        CoinManager.Instance.CoinCollected.AddListener(GenerateNewDestination);
        target = FindObjectOfType<PlayerScript>();
    }

    protected override void Update()
    {
        base.Update();
        if (!isActive || !alive)
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
                }
            }
            else if (Vector3.Distance(transform.position - new Vector3(0f, transform.position.y, 0f), agent.destination - new Vector3(0f, agent.destination.y, 0f)) <= agent.stoppingDistance + 1f)
            {
                GenerateNewDestination();
            }
        }
        if (currentSpecialAttack != Nothing && ShouldUltimate(currentSpecialAttack))
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
        if (!isActive || GameManager.Instance.Paused || (hit.collider != null && hit.transform.CompareTag("Player")))
            return;
        agent.stoppingDistance = 0f;
        int coinIndex = Random.Range(0, CoinManager.Instance.Coins.Length);
        float coinDistance = Mathf.Infinity;
        for (int i = 0; i < CoinManager.Instance.Coins.Length; i++)
        {
            if (CoinManager.Instance.Coins[i].activeSelf && Vector3.Distance(transform.position, CoinManager.Instance.Coins[i].transform.position) < coinDistance)
            {
                coinDistance = Vector3.Distance(transform.position, CoinManager.Instance.Coins[i].transform.position);
                coinIndex = i;
            }
        }
        agent.destination = CoinManager.Instance.Coins[coinIndex].transform.position;
    }

    protected bool ShouldUltimate(SpecialAttackMethod currentUlt)
    {
        if (currentUlt == null || currentUlt == Nothing)      //Not allowed to use switch on delegate methods :c ugly code it is!
        {
            return false;
        }
        else if (Time.time > ultStartedTime + specialAttackTimer - 1)
        {
            return true;
        }
        else if (currentUlt == SuperHeal || currentUlt == Heal)
        {
            if (health <= (maxHealth / 3) * 4)
                return true;
            return false;
        }
        else if (currentUlt == SuperShots || currentUlt == SpawnShield)
        {
            RaycastHit hit;
            if (Physics.Raycast(shotStart.transform.position, (target.transform.position - shotStart.transform.position), out hit))
            {
                if (hit.transform.GetComponent<PlayerScript>() != null)
                {
                    return true;
                }
            }
            return false;
        }
        else if (currentUlt == DeployMine)
        {
            if (Vector3.Distance(transform.position, agent.destination) < 10f)
                return true;
            return false;
        }
        return true;
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

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (!alive)
        {
            agent.isStopped = true;
        }
    }

    public void DeActivate()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        if (engineSound != null)
            AudioManager.Instance.ReturnSource(engineSound);
        agent.enabled = false;
        health = maxHealth;
        targetHealth = maxHealth;
        coins = 0;
        isActive = false;
        alive = false;
    }

    public void Activate()
    {
        health = maxHealth;
        alive = true;
        healthSlider.value = healthSlider.maxValue;
        healthSliderDelayed.value = healthSliderDelayed.maxValue;
        isActive = true;
        agent.enabled = true;
        agent.isStopped = false;
        engineSound = AudioManager.Instance.SpawnSound("EngineSound", transform, false, true, false, 0.275f);
    }

    public bool IsActive()
    {
        return isActive;
    }
}