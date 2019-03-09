using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : TankScript
{
    Vector3 position;

    Rigidbody rB;

    protected override void Awake()
    {
        base.Awake();
        rB = GetComponent<Rigidbody>();
    }

    public Vector3 Position
    {
        get { return position; }
    }

    protected override void MoveTank(float amount)
    {
        if (!alive)
            return;
        rB.AddForce(Vector3.forward * amount);
    }

    private void Update()
    {
        if (!alive)
        {
            return;
        }

        position = transform.position;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentRotationMethod = RotateTower;
            currentMovement = DontMoveTank;
        }
        else
        {
            currentRotationMethod = RotateTank;
            currentMovement = MoveTank;
        }

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            currentMovement(speed);
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            currentMovement(-speed);
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            currentRotationMethod(-turnSpeed);
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            currentRotationMethod(turnSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E) && coins >= 5)
        {
            coins -= 5;
            StopCoroutine("SpecialAttackTimer");
            StartCoroutine("SpinWheel");
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentSpecialAttack != Nothing)
        {
            currentSpecialAttack();
            StopCoroutine("SpecialAttackTimer");
            currentSpecialAttack = Nothing;
        }
    }
}