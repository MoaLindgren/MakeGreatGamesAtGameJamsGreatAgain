using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]
public class BlastRadiusScript : MonoBehaviour
{
    AudioSource aS;

    SphereCollider coll;

    void Start()
    {
        aS = GetComponent<AudioSource>();
        coll = GetComponent<SphereCollider>();
    }
}