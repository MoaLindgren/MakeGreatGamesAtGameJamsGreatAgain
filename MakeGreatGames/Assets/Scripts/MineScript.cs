using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MonoBehaviour, IPoolable
{
    [SerializeField]
    int damage;

    [SerializeField]
    GameObject explosion;

    Material mat;

    float sinPos = 0f, colorAmount = 127.5f;

    bool mineActivated = false, active = false;

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().materials[0];
    }

    void Update()
    {
        if (!active || GameManager.Instance.Paused)
            return;
        sinPos += Time.deltaTime;
        colorAmount = 0.5f + (Mathf.Lerp(-1, 1, Mathf.Sin(sinPos)) / 2);
        mat.SetColor("_EmissionColor", new Color(colorAmount, colorAmount, colorAmount));
        if(colorAmount == 0f)
        {
            sinPos = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!mineActivated || !active)
            return;
        TankScript tank = other.GetComponent<TankScript>();
        if (tank != null)
        {
            tank.TakeDamage(damage);
            GameObject exp = Instantiate(explosion);
            AudioManager.Instance.SpawnSound("ExplosionSound", exp.transform, true, false, false, 0.658f);
            GameManager.Instance.MinePool.RePoolObject(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TankScript tank = other.GetComponent<TankScript>();
        if(!mineActivated && tank != null)
        {
            mineActivated = true;
        }
    }

    public void Activate()
    {
        active = true;
    }

    public bool IsActive()
    {
        return active;
    }

    public void DeActivate()
    {
        active = false;
        mineActivated = false;
    }
}