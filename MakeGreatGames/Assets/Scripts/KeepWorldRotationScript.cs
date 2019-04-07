using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepWorldRotationScript : MonoBehaviour
{
    [SerializeField]
    bool local = false;

    void Update()
    {
        if (!local)
            transform.rotation = Quaternion.identity;
        else
            transform.localRotation = Quaternion.identity;
    }
}