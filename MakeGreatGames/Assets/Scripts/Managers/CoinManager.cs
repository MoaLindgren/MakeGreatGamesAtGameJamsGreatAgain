using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class CoinManager : NetworkBehaviour
{
    [SerializeField]
    GameObject[] coins;     //Coin locations

    [SyncVar]
    [SerializeField]
    float minCoinTime, maxCoinTime;     //Min and max time between coin spawns

    [SerializeField]
    int coinsToUlt;

    static CoinManager instance;

    bool onNetwork;

    [SyncVar]
    float waitTime, lastSpawn;

    public static CoinManager Instance
    {
        get { return instance; }
    }

    public int CoinsToUlt
    {
        get { return coinsToUlt; }
    }

    public GameObject[] Coins
    {
        get { return coins; }
    }

    int activeCoins = 0;

    public UnityEvent CoinSpawned = new UnityEvent(), CoinCollected = new UnityEvent();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        onNetwork = GetComponent<NetworkIdentity>() != null;
        print("Hello!");
        if (onNetwork)
        {
            lastSpawn = Time.time;
            CmdSetSpawnTimer();
        }
        else
            StartCoroutine("SpawnCoin");
    }

    private void Update()
    {
        if (!onNetwork)
            return;
        if (Time.time >= lastSpawn + waitTime)
        {
            if (activeCoins < coins.Length)
            {
                bool spawnChosen = true;
                foreach (GameObject GO in coins)     //Avoid inf loops
                    if (!GO.activeSelf)
                    {
                        spawnChosen = false;
                        break;
                    }
                while (!spawnChosen)
                {
                    int index = Random.Range(0, coins.Length);
                    if (!coins[index].activeSelf)
                    {
                        NewCoin(index);
                        spawnChosen = true;
                        CoinSpawned.Invoke();
                    }
                }
            }
            lastSpawn = Time.time;
            if (onNetwork)
                CmdSetSpawnTimer();
            else
                StartCoroutine("SpawnCoin");
        }
    }

    [Command]
    void CmdSetSpawnTimer()
    {
        waitTime = Time.time + Random.Range(minCoinTime, maxCoinTime);
    }

    void StartSpawning()
    {
        StartCoroutine("SpawnCoin");
    }

    public void CoinPickedUp(GameObject coin)
    {
        coin.SetActive(false);
        activeCoins--;
        CoinCollected.Invoke();
    }

    [ClientRpc]
    public void RpcCoinPickedUp(GameObject coin)
    {
        coin.SetActive(false);
        activeCoins--;
        CoinCollected.Invoke();
    }

    IEnumerator SpawnCoin()
    {
        yield return new WaitForSeconds(Random.Range(minCoinTime, maxCoinTime));
        if (activeCoins < coins.Length)
        {
            bool spawnChosen = true;
            foreach (GameObject GO in coins)     //Avoid inf loops
                if (!GO.activeSelf)
                {
                    spawnChosen = false;
                    break;
                }
            while (!spawnChosen)
            {
                int index = Random.Range(0, coins.Length);
                if (!coins[index].activeSelf)
                {
                    NewCoin(index);
                    spawnChosen = true;
                    CoinSpawned.Invoke();
                }
            }
        }
        StartCoroutine("SpawnCoin");
    }

    void NewCoin(int index)
    {
        print("Coin!");
        coins[index].SetActive(true);
        AudioManager.Instance.SpawnSound("CoinSpawnSound", coins[index].transform, true, false, false, 1f);
        activeCoins++;
    }

    [ClientRpc]
    void RpcNewCoin(int index)
    {
        coins[index].SetActive(true);
        AudioManager.Instance.SpawnSound("CoinSpawnSound", coins[index].transform, true, false, false, 1f);
        activeCoins++;
    }
}