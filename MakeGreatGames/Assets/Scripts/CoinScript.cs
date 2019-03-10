﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class CoinScript : MonoBehaviour
{
    [SerializeField]
    float spinAmount;

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

    void Update()
    {
        transform.Rotate(0f, spinAmount, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript tank = other.GetComponent<TankScript>();
        if (tank != null)
        {
            tank.AddCoin();
            CoinManager.Instance.CoinPickedUp(gameObject);
        }
    }
}