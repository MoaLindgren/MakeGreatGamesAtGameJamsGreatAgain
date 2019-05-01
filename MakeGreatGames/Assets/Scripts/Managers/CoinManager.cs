using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class CoinManager : NetworkBehaviour
{
    [SerializeField]
    GameObject[] coins;     //Coin locations

    [SerializeField]
    float minCoinTime, maxCoinTime;     //Min and max time between coin spawns

    [SerializeField]
    int coinsToUlt;
    
    static CoinManager instance;

    bool onNetwork;

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
        print("Waiting......");
        yield return new WaitForSeconds(Random.Range(minCoinTime, maxCoinTime));
        print("Done waiting!");
        CoinSpawned.Invoke();
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
        if (onNetwork)
        {
            print("Online Coin!");
            RpcNewCoin(index);
        }
    }

    [ClientRpc]
    void RpcNewCoin(int index)
    {
        coins[index].SetActive(true);
        AudioManager.Instance.SpawnSound("CoinSpawnSound", coins[index].transform, true, false, false, 1f);
        activeCoins++;
    }
}