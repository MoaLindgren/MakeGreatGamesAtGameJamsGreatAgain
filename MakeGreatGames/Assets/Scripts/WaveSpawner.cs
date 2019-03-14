using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField]
    float timeBetweenSpawns, timeBetweenWaves;

    [SerializeField]
    Transform[] spawnPoints;

    static WaveSpawner instance;

    public static WaveSpawner Instance
    {
        get { return instance; }
    }

    int currentWave = 0, spawnPointIndex = 0, remainingEnemies;

    List<GameObject> currentWaveTanks = new List<GameObject>();

    bool waveSpawned = false;

    public List<GameObject> CurrentWaveTanks
    {
        get { return currentWaveTanks; }
    }

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
        StartCoroutine("SpawnWave");
    }

    IEnumerator SpawnWave()
    {
        waveSpawned = false;
        currentWave++;
        remainingEnemies = currentWave;
        for (int i = 0; i < currentWave && i < 100; i++)
        {
            currentWaveTanks.Add(GameManager.Instance.EnemyPool.GetObject(spawnPoints[spawnPointIndex].transform.position, spawnPoints[spawnPointIndex].transform.rotation));
            spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Length;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        waveSpawned = true;
    }
    
    public void TankDestroyed(GameObject tank)
    {
        if (currentWaveTanks.Contains(tank))
            currentWaveTanks.Remove(tank);
        GameManager.Instance.EnemyPool.RePoolObject(tank);
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