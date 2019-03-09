using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] coins;

    static CoinManager instance;

    public static CoinManager Instance
    {
        get { return instance; }
    }

    int activeCoins = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    public void CoinPickedUp(GameObject coin)
    {
        coin.SetActive(false);
        activeCoins--;
    }

    IEnumerator SpawnCoin()
    {
        yield return new WaitForSeconds(Random.Range(10, 60));
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