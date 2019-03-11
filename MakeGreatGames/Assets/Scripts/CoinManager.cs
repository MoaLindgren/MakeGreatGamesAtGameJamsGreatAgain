using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CoinManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] coins;     //Coin locations

    [SerializeField]
    float minCoinTime, maxCoinTime;     //Min and max time between coin spawns

    [SerializeField]
    AudioSource aS;     //Sound playing when a coin is picked up

    static CoinManager instance;

    public static CoinManager Instance
    {
        get { return instance; }
    }

    public GameObject[] Coins
    {
        get { return coins; }
    }

    int activeCoins = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        StartCoroutine("SpawnCoin");
    }

    public void CoinPickedUp(GameObject coin)
    {
        coin.SetActive(false);
        aS.Play();
        activeCoins--;
    }

    IEnumerator SpawnCoin()
    {
        yield return new WaitForSeconds(Random.Range(minCoinTime, maxCoinTime));
        if (activeCoins < coins.Length)
        {
            bool spawnChosen = false;
            while (!spawnChosen)
            {
                int index = Random.Range(0, coins.Length);
                if (!coins[index].activeSelf)
                {
                    coins[index].SetActive(true);
                    activeCoins++;
                    spawnChosen = true;
                }
            }
        }
        StartCoroutine("SpawnCoin");
    }
}