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
    LineRenderer line;
    UiManager uiManager;

    protected override void Awake()
    {
        base.Awake();
        rB = GetComponent<Rigidbody>();
        uiManager = FindObjectOfType<UiManager>();
    }

    public Vector3 Position
    {
        get { return position; }
    }

    public override void AddCoin()
    {
        base.AddCoin();
        uiManager.Coins = coins;
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
            RaycastHit hit;
            if (Physics.Raycast(shotStart.transform.position, shotStart.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                Debug.DrawRay(shotStart.transform.position, shotStart.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                line.SetPosition(0, shotStart.transform.position);
                line.SetPosition(1, hit.point);
                line.enabled = true;
            }
        }

        else
        {
            currentRotationMethod = RotateTank;
            currentMovement = MoveTank;
            line.enabled = false;
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
            currentRotationMethod(-towerTurnSpeed);
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            
            currentRotationMethod(towerTurnSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            CameraShaker.Instance.ShakeCamera(shotDamage * cameraShakeShoot, 0.5f);
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E) && coins >= 3)
        {
            coins -= 3;
            uiManager.Coins = coins;
            StopCoroutine("SpecialAttackTimer");
            StartCoroutine("SpinWheel");
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentSpecialAttack != Nothing)
        {
            currentSpecialAttack();
            StopCoroutine("SpecialAttackTimer");
            uiManager.SpecialAttack(false, 0, 0);
            currentSpecialAttack = Nothing;
        }
    }
}