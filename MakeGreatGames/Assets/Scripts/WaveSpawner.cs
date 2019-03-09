using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject tank;

    [SerializeField]
    Transform[] spawnPoints;

    static WaveSpawner instance;

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
        for (int i = 0; i < 100; i++)
        {
            enemyPool.Add(Instantiate(tank, new Vector3(10000, 10000, 10000), Quaternion.identity));
        }
    }

    IEnumerator SpawnWave()
    {
        yield return null;
    }
}
