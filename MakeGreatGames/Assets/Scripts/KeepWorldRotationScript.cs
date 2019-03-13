using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepWorldRotationScript : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
