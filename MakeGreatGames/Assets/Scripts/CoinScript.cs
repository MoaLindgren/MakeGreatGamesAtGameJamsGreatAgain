using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class CoinScript : MonoBehaviour
{
    [SerializeField]
    GameObject coinPickupParticles;

    ParticleSystem[] spawnParticles;

    private void Awake()
    {
        if (spawnParticles == null || spawnParticles.Length < 1)
            spawnParticles = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem p in spawnParticles)
        {
            p.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript tank = other.GetComponent<TankScript>();
        if (tank != null)
        {
            tank.AddCoin();
            CoinManager.Instance.CoinPickedUp(gameObject);

            Instantiate(coinPickupParticles, transform.position, new Quaternion(-90f, 0f, 0f, 0f));
        }
    }
}