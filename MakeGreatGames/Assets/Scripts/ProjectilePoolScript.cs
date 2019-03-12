using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolScript : MonoBehaviour
{
    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    int poolSize;

    static ProjectilePoolScript instance;

    public static ProjectilePoolScript Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        CreatePool();
    }
    
    void Update()
    {
        
    }

    void CreatePool()
    {
        for(int i = 0; i < poolSize; i++)
        {

        }
    }
}