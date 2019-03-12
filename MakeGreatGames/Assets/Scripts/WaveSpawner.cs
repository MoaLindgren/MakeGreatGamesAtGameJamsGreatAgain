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

    int currentWave = 0, poolIndex = 0, spawnPointIndex = 0, remainingEnemies;

    List<GameObject> currentWaveTanks = new List<GameObject>(), enemyPool = new List<GameObject>();

    bool waveSpawned = false;

    public int CurrentWave
    {
        get { return currentWave; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    private void Start()
    {
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
        waveSpawned = false;
        currentWave++;
        remainingEnemies = currentWave;
        for (int i = 0; i < currentWave && i < 100; i++)
        {
            SpawnTank();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        waveSpawned = true;
    }

    void SpawnTank()
    {
        while (enemyPool[poolIndex].activeSelf)
        {
            poolIndex = (poolIndex + 1) % enemyPool.Count;
        }
        enemyPool[poolIndex].transform.position = spawnPoints[spawnPointIndex].position;
        enemyPool[poolIndex].SetActive(true);
        currentWaveTanks.Add(enemyPool[poolIndex]);
        enemyPool[poolIndex].GetComponent<NpcScript>().SetAlive(true);
        poolIndex = (poolIndex + 1) % enemyPool.Count;
        spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Length;
    }

    public void TankDestroyed(GameObject tank)
    {
        if (currentWaveTanks.Contains(tank))
            currentWaveTanks.Remove(tank);
        tank.GetComponent<NpcScript>().SetAlive(false);
        tank.SetActive(false);
        tank.transform.position = new Vector3(10000, 10000, 10000);
        remainingEnemies--;
        if (currentWaveTanks.Count < 1 && waveSpawned)
        {
            UiManager.Instance.AddScore(50 * currentWave);
            GameManager.Instance.Score = 50 * currentWave;
            StartCoroutine("NextWave");
        }
    }

    IEnumerator NextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine("SpawnWave");
    }
}