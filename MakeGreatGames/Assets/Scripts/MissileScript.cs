using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
public class MissileScript : MonoBehaviour, IPoolable
{
    [SerializeField]
    int damage;

    [SerializeField]
    GameObject blast;

    TankScript target, shooter;

    NavMeshAgent agent;

    Material mat;

    bool active, onNetwork;

    float sinPos = 0f, colorAmount = 127.5f;

    float[] emissionRates;

    ParticleSystem[] particles;

    private void Awake()
    {
        onNetwork = GetComponent<NetworkTransform>() == null ? false : true;
        particles = GetComponentsInChildren<ParticleSystem>();
        emissionRates = new float[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            emissionRates[i] = particles[i].emissionRate;
        }
        agent = GetComponent<NavMeshAgent>();
        if (agent.destination == null)
        {
            agent.destination = CoinManager.Instance.Coins[Random.Range(0, CoinManager.Instance.Coins.Length)].transform.position;
        }
        mat = GetComponentInChildren<MeshRenderer>().materials[0];
    }

    public void Init(TankScript target, TankScript shooter)
    {
        AudioManager.Instance.SpawnSound("MissileTravelSound", transform, false, false, false, 0.8f);
        this.target = target;
        this.shooter = shooter;
        if (target != null)
            agent.destination = target.transform.position;
    }

    void Update()
    {
        if (!active || GameManager.Instance.Paused)
            return;
        sinPos += Time.deltaTime;
        colorAmount = 0.5f + (Mathf.Lerp(-1, 1, Mathf.Sin(sinPos)) / 2);
        mat.SetColor("_EmissionColor", new Color(colorAmount, colorAmount, colorAmount));
        if (colorAmount == 0f)
        {
            sinPos = 0f;
        }
        if (target == null || (target is NpcScript && !(target as NpcScript).IsActive()))
        {
            if (!onNetwork)
            {
                if (WaveSpawner.Instance.CurrentWaveTanks.Count > 0)
                {
                    target = WaveSpawner.Instance.CurrentWaveTanks[Random.Range(0, WaveSpawner.Instance.CurrentWaveTanks.Count)].GetComponent<TankScript>();
                }
                else
                {
                    target = null;
                    Vector3 newTarget = CoinManager.Instance.Coins[Random.Range(0, CoinManager.Instance.Coins.Length)].transform.position;
                    newTarget.y = transform.position.y;
                    agent.destination = newTarget;
                }
            }
            else
            {
                PlayerScript[] allPlayers = FindObjectsOfType<PlayerScript>();
                bool targetAcquired = false;
                int index;
                while (!targetAcquired)
                {
                    index = Random.Range(0, allPlayers.Length);
                    if(allPlayers[index] != shooter)
                    {
                        target = allPlayers[index];
                        targetAcquired = true;
                    }
                }
            }
        }
        if (target != null)
            agent.destination = target.transform.position;
        else if (Vector3.Distance(transform.position, agent.destination) < 0.1f)
        {
            Instantiate(blast, transform.position, Quaternion.identity);
            GameManager.Instance.MissilePool.RePoolObject(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript hitTank = other.GetComponent<TankScript>();
        IPoolable hitPoolable = other.GetComponent<IPoolable>();
        if (hitTank != null && hitTank != shooter)
        {
            hitTank.TakeDamage(damage);
            GameObject exp = Instantiate(blast, transform.position, Quaternion.identity);
            AudioManager.Instance.SpawnSound("ExplosionSound", exp.transform, true, false, false, 0.658f);
            GameManager.Instance.MissilePool.RePoolObject(gameObject);
        }
        else if (hitPoolable != null)
        {
            GameManager.Instance.MissilePool.RePoolObject(gameObject);
        }
    }

    public bool IsActive()
    {
        return active;
    }

    public void Activate()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].emissionRate = emissionRates[i];
        }
        agent.enabled = true;
        active = true;
    }

    public void DeActivate()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].emissionRate = 0f;
        }
        agent.enabled = false;
        active = false;
    }
}