using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject tank;

    [SerializeField]
    float timeBetweenSpawns, timeBetweenWaves;

    [SerializeField]
    Transform[] spawnPoints;

    static WaveSpawner instance;

    int wave = 0, poolIndex = 0, spawnPointIndex = 0;

    public static WaveSpawner Instance
    {
        get { return instance; }
    }

    List<GameObject> currentWave = new List<GameObject>();

    List<GameObject> enemyPool = new List<GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        for (int i = 0; i < 1000; i++)
        {
            GameObject spawnedTank = Instantiate(tank, new Vector3(10000, 10000, 10000), Quaternion.identity);
            enemyPool.Add(spawnedTank);
            spawnedTank.SetActive(false);
        }
    }

    IEnumerator SpawnWave()
    {
        wave++;
        for (int i = 0; i < wave; i++)
        {
            SpawnTank();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void SpawnTank()
    {
        while (enemyPool[poolIndex].activeSelf)
        {
            poolIndex = (poolIndex + 1) % enemyPool.Count;
        }
        enemyPool[poolIndex].transform.position = spawnPoints[spawnPointIndex].position;
        enemyPool[poolIndex].SetActive(true);
        //enemyPool[poolIndex].GetComponent<NpcScript>().
        poolIndex = (poolIndex + 1) % enemyPool.Count;
    }

    public void TankDestroyed(GameObject tank)
    {
        if (currentWave.Contains(tank))
            currentWave.Remove(tank);
        tank.SetActive(false);
        tank.transform.position = new Vector3(10000, 10000, 10000);
        if (currentWave.Count < 1)
        {
            StartCoroutine("NextWave");
        }
    }

    IEnumerator NextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine("SpawnWave");
    }
}