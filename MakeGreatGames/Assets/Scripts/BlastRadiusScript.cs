using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]
public class BlastRadiusScript : MonoBehaviour
{
    AudioSource aS;

    SphereCollider coll;

    [SerializeField]
    int damage;

    void Start()
    {
        aS = GetComponent<AudioSource>();
        coll = GetComponent<SphereCollider>();
        Destroy(gameObject, 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript tank = other.GetComponent<TankScript>();
        if (tank != null)
            tank.TakeDamage(damage);
    }
}