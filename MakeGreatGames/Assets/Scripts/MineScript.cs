using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MineScript : MonoBehaviour
{
    [SerializeField]
    int damage;

    [SerializeField]
    GameObject explosion;

    Material mat;

    float sinPos = 0f, colorAmount = 127.5f;

    bool active = false;

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().materials[0];
    }

    void Update()
    {
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
        if (!active)
            return;
        TankScript tank = other.GetComponent<TankScript>();
        if (tank != null)
        {
            Instantiate(explosion);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TankScript tank = other.GetComponent<TankScript>();
        if(!active && tank != null)
        {
            active = true;
        }
    }
}