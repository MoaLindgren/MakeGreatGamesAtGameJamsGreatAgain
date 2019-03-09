using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : TankScript
{
    Vector3 position;

    public Vector3 Position
    {
        get { return position; }
    }

    private void Start()
    {
        position = transform.position;
    }
}
