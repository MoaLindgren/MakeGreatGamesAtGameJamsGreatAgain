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

    float rotationCompensation = 0f;

    public float RotationCompensation
    {
        get
        {
            return rotationCompensation;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        rB = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();
        engineSound = AudioManager.Instance.SpawnSound("EngineSound", transform, false, true, false, 1f);
    }

    public override void AddCoin()
    {
        base.AddCoin();
        UIManager.Instance.Coins = coins;
    }

    protected override void MoveTank(float amount)
    {
        base.MoveTank(amount);
        if (!alive)
            return;
        rB.MovePosition(transform.position + amount * transform.forward * Time.deltaTime);
    }

    protected override void Update()
    {
        base.Update();
        if (!alive)
        {
            return;
        }

        if (Input.GetButton("Line"))
        {
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
            line.enabled = false;
        }

        if (Input.GetAxis("Fire") > 0.5f && canShoot)
        {
            CameraShaker.Instance.ShakeCamera(shotDamage * cameraShakeShoot, 0.5f);
            Shoot();
        }

        if (Input.GetButtonDown("Spin") && coins >= CoinManager.Instance.CoinsToUlt)
        {
            coins -= CoinManager.Instance.CoinsToUlt;
            UIManager.Instance.Coins = coins;
            StopCoroutine("SpecialAttackTimer");
            StartCoroutine("SpinWheel");
        }
        
        if (Input.GetButtonDown("Ultimate") && currentSpecialAttack != Nothing)
        {
            currentSpecialAttack();
            StopCoroutine("SpecialAttackTimer");
            UIManager.Instance.SpecialAttack(false, 0, 0);
            currentSpecialAttack = Nothing;
        }
    }

    private void FixedUpdate()
    {
        if (!alive)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("TankHorizontal"), vertical = Input.GetAxisRaw("TankVertical");

        rB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (vertical > 0f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, acceleration * vertical);
            foreach (ParticleSystem p in backSmoke)
            {
                p.emissionRate = 0;
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.emissionRate = 20;
            }
        }
        else if (vertical < 0f)
        {
            speed = Mathf.Lerp(speed, -maxSpeed * 0.7f, acceleration * Mathf.Abs(vertical));
            foreach (ParticleSystem p in backSmoke)
            {
                p.emissionRate = 20;
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.emissionRate = 0;
            }
        }
        else
        {
            speed = Mathf.Lerp(speed, 0f, acceleration * 3f);
            foreach (ParticleSystem p in backSmoke)
            {
                p.emissionRate = 0;
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.emissionRate = 0;
            }
            rB.velocity = Vector3.zero;
            if (Mathf.Abs(speed) < acceleration * 40f)
            {
                speed = 0f;
                rB.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        speed = Mathf.Clamp(speed, -maxSpeed * 0.7f, maxSpeed);
        currentMovement(speed);
        if (horizontal < -0.19f)
        {
            RotateTank(-turnSpeed * Mathf.Abs(horizontal));
            rotationCompensation = -turnSpeed;
        }
        else if (horizontal > 0.19f)
        {
            RotateTank(turnSpeed * Mathf.Abs(horizontal));
            rotationCompensation = turnSpeed;
        }
        else
        {
            rotationCompensation = 0f;
        }
        float aim = Input.GetAxis("Aim");
        if (Mathf.Abs(aim) > 0.19f)
        {
            RotateTower(towerTurnSpeed * aim);
        }
    }
}