using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

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
    int poolSize, lobbyIndex;

    [SerializeField]
    bool isOnline;

    List<GameObject> objectPool = new List<GameObject>();

    int poolIndex = 0;

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            objectPool.Add(Instantiate(prefab, transform.position, Quaternion.identity));
            if (isOnline)
            {
                NetworkServer.Spawn(objectPool[objectPool.Count - 1]);
            }
            RenderGO(objectPool[i], false);
            objectPool[i].GetComponent<IPoolable>().DeActivate();
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        int startIndex = poolIndex;
        bool instantiate = false;
        while (objectPool[poolIndex].GetComponent<IPoolable>().IsActive())
        {
            poolIndex = (poolIndex + 1) % objectPool.Count;
            if (poolIndex == startIndex)     //Avoids inf loops
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
        else if (isOnline)
        {
            NetworkServer.Spawn(returnGO);
        }
        RenderGO(returnGO, true);
        returnGO.GetComponent<IPoolable>().Activate();
        poolIndex = (poolIndex + 1) % objectPool.Count;
        return returnGO;
    }

    public void RePoolObject(GameObject GO)
    {
        if (!objectPool.Contains(GO))
            objectPool.Add(GO);
        GO.transform.position = transform.position;
        RenderGO(GO, false);
        GO.GetComponent<IPoolable>().DeActivate();
    }

    void RenderGO(GameObject GO, bool render)
    {
        if (GO.GetComponent<Renderer>() != null)
            GO.GetComponent<Renderer>().enabled = render;
        foreach (Renderer r in GO.GetComponentsInChildren<Renderer>())
            if (!(r is ParticleSystemRenderer))
                r.enabled = render;
        if (!render)
        {
            if (GO.GetComponent<AudioSource>() != null)
                GO.GetComponent<AudioSource>().Stop();
            foreach (AudioSource aS in GO.GetComponentsInChildren<AudioSource>())
                aS.Stop();
        }
    }
}