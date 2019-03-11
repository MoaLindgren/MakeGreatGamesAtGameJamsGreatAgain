using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : TankScript
{
    [SerializeField]
    LineRenderer line;

    Rigidbody rB;

    protected override void Awake()
    {
        base.Awake();
        rB = GetComponent<Rigidbody>();
    }

    public override void AddCoin()
    {
        base.AddCoin();
        UiManager.Instance.Coins = coins;
    }

    protected override void MoveTank(float amount)
    {
        if (!alive)
            return;
        rB.MovePosition(transform.position + amount * transform.forward * Time.deltaTime);
    }

    private void Update()
    {
        if (!alive)
        {
            return;
        }

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canShoot)
            {
                CameraShaker.Instance.ShakeCamera(shotDamage * cameraShakeShoot, 0.5f);
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && coins >= 3)
        {
            coins -= 3;
            UiManager.Instance.Coins = coins;
            StopCoroutine("SpecialAttackTimer");
            StartCoroutine("SpinWheel");
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentSpecialAttack != Nothing)
        {
            currentSpecialAttack();
            StopCoroutine("SpecialAttackTimer");
            UiManager.Instance.SpecialAttack(false, 0, 0);
            currentSpecialAttack = Nothing;
        }
    }

    private void FixedUpdate()
    {
        if (!alive)
        {
            return;
        }

        rB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            foreach (ParticleSystem p in backSmoke)
            {
                p.Stop();
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.Play();
            }
            currentMovement(speed);
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            foreach (ParticleSystem p in backSmoke)
            {
                p.Play();
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.Stop();
            }
            currentMovement(-speed);
        }
        else
        {
            foreach (ParticleSystem p in backSmoke)
            {
                p.Stop();
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.Stop();
            }
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
    }
}