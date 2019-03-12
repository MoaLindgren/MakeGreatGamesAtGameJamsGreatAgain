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

    GameObject[] projectilePool;

    int poolIndex = 0;

    public static ProjectilePoolScript Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        projectilePool = new GameObject[poolSize];
        CreatePool();
    }

    void CreatePool()
    {
        for(int i = 0; i < poolSize; i++)
        {
            GameObject spawnedProjectile = Instantiate(projectilePrefab, new Vector3(-10000, -10000, -10000), Quaternion.identity);
            projectilePool[i] = spawnedProjectile;
        }
    }

    public GameObject NewProjectile()
    {
        while (projectilePool[poolIndex].GetComponent<ProjectileScript>().Active)
        {
            poolIndex = (poolIndex + 1) % projectilePool.Length;
        }
        projectilePool[poolIndex].GetComponent<ProjectileScript>().Active = true;
        projectilePool[poolIndex].GetComponent<ProjectileScript>().Render(true);
        return projectilePool[poolIndex];
    }

    public void ProjectileDestroyed(GameObject projectileGO)
    {
        ProjectileScript projectile = projectileGO.GetComponent<ProjectileScript>();
        projectileGO.transform.position = new Vector3(-10000, -10000, -10000);
        projectile.Active = false;
        projectile.Render(false);
    }
}