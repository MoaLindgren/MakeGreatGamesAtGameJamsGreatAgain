using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IPoolable
{
    void DeActivate();
    void Activate();
    bool IsActive();
}

public class PoolScript : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    [SerializeField]
    int poolSize;

    GameObject[] objectPool;

    int poolIndex = 0;

    void Awake()
    {
        objectPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            objectPool[i] = Instantiate(prefab, transform.position, Quaternion.identity);
            objectPool[i].SetActive(false);
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        int startIndex = poolIndex;
        bool instantiate = false;
        while (objectPool[poolIndex].activeSelf)
        {
            poolIndex = (poolIndex + 1) % objectPool.Length;
            if (poolIndex == startIndex)     //Avoid inf loops
            {
                instantiate = true;
                break;
            }
        }
        GameObject returnGO = instantiate ? Instantiate(prefab, position, rotation) : objectPool[poolIndex];
        if (!instantiate)
        {
            returnGO.transform.position = position;
            returnGO.transform.rotation = rotation;
        }
        returnGO.SetActive(true);
        returnGO.GetComponent<IPoolable>().Activate();
        poolIndex = (poolIndex + 1) % objectPool.Length;
        return returnGO;
    }

    public void RePoolObject(GameObject GO)
    {
        if (Array.IndexOf(objectPool, GO) > -1)
        {
            GO.transform.position = transform.position;
            GO.GetComponent<IPoolable>().DeActivate();
            GO.SetActive(false);
        }
        else
        {
            Destroy(GO);
        }
    }
}