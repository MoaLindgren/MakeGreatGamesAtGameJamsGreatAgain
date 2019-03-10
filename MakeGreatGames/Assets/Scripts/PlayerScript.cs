using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : TankScript
{
    Vector3 position;

    Rigidbody rB;

    [SerializeField]
    //Image 

    protected override void Awake()
    {
        base.Awake();
        rB = GetComponent<Rigidbody>();
    }

    public Vector3 Position
    {
        get { return position; }
    }

    public override void AddCoin()
    {
        base.AddCoin();
        print(coins + " coins");
        //uppdatera cointext
    }

    protected override void MoveTank(float amount)
    {
        if (!alive)
            return;
        rB.MovePosition(transform.position + amount * transform.forward * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!alive)
        {
            return;
        }

        position = transform.position;
        rB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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
        else
        {
            rB.velocity = Vector3.zero;
            rB.constraints = RigidbodyConstraints.FreezeAll;
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

        if (Input.GetKeyDown(KeyCode.E) && coins >= 3)
        {
            coins -= 3;
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