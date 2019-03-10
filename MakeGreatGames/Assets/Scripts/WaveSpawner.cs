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

    public static WaveSpawner Instance
    {
        get { return instance; }
    }

    int wave = 0, poolIndex = 0, spawnPointIndex = 0;

    List<GameObject> currentWave = new List<GameObject>(), enemyPool = new List<GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        for (int i = 0; i < 100; i++)
        {
            GameObject spawnedTank = Instantiate(tank, new Vector3(10000, 10000, 10000), Quaternion.identity);
            enemyPool.Add(spawnedTank);
            spawnedTank.GetComponent<NpcScript>().SetAlive(false);
            spawnedTank.SetActive(false);
        }
        StartCoroutine("SpawnWave");
    }

    IEnumerator SpawnWave()
    {
        if (wave < 100)
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
        enemyPool[poolIndex].GetComponent<NpcScript>().SetAlive(true);
        poolIndex = (poolIndex + 1) % enemyPool.Count;
        spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Length;
    }

    public void TankDestroyed(GameObject tank)
    {
        if (currentWave.Contains(tank))
            currentWave.Remove(tank);
        tank.GetComponent<NpcScript>().SetAlive(false);
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