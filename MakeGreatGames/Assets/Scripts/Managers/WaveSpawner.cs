using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField]
    float timeBetweenSpawns, timeBetweenWaves;

    [SerializeField]
    Transform[] spawnPoints;

    int currentWave = 0, spawnPointIndex = 0, remainingEnemies;

    float bonusPoints = 0;

    List<GameObject> currentWaveTanks = new List<GameObject>();

    bool waveSpawned = false;

    static WaveSpawner instance;

    public static WaveSpawner Instance
    {
        get { return instance; }
    }

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

    private void Update()
    {
        if (waveSpawned)
        {
            bonusPoints = Mathf.Clamp(bonusPoints - Time.deltaTime, 0f, Mathf.Infinity);
            print(bonusPoints);
        }
    }

    IEnumerator SpawnWave()
    {
        waveSpawned = false;
        currentWave++;
        print("WAVE: " + currentWave);
        remainingEnemies = currentWave;
        for (int i = 0; i < currentWave && i < 100; i++)
        {
            currentWaveTanks.Add(GameManager.Instance.EnemyPool.GetObject(spawnPoints[spawnPointIndex].transform.position, spawnPoints[spawnPointIndex].transform.rotation));
            spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Length;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        bonusPoints = 60f * currentWave;
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
            UIManager.Instance.AddScore((int)bonusPoints);
            GameManager.Instance.Score = (int)bonusPoints;
            StartCoroutine("NextWave");
        }
    }

    IEnumerator NextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine("SpawnWave");
    }
}