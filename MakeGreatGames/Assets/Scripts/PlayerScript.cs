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

    private void Start()
    {
        engineSound = AudioManager.Instance.SpawnSound("EngineSound", transform, false, true, false, 0.612f);
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

    private void Update()
    {
        if (!alive)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentRotationMethod = RotateTower;
            //currentMovement = DontMoveTank;
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
            //currentMovement = MoveTank;
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

        if (Input.GetKeyDown(KeyCode.E) && coins >= CoinManager.Instance.CoinsToUlt)
        {
            coins -= CoinManager.Instance.CoinsToUlt;
            UIManager.Instance.Coins = coins;
            StopCoroutine("SpecialAttackTimer");
            StartCoroutine("SpinWheel");
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentSpecialAttack != Nothing)
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

        rB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (Input.GetAxisRaw("Vertical") > 0f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, acceleration);
            foreach (ParticleSystem p in backSmoke)
            {
                p.emissionRate = 0;
            }
            foreach (ParticleSystem p in frontSmoke)
            {
                p.emissionRate = 20;
            }
        }
        else if (Input.GetAxisRaw("Vertical") < 0f)
        {
            speed = Mathf.Lerp(speed, -maxSpeed * 0.7f, acceleration);
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
        float turnAmount = currentRotationMethod == RotateTank ? turnSpeed : towerTurnSpeed;
        if (Input.GetKey(KeyCode.A))
        {
            currentRotationMethod(-turnAmount);
            rotationCompensation = -turnSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentRotationMethod(turnAmount);
            rotationCompensation = turnSpeed;
        }
        else
        {
            rotationCompensation = 0f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotateTower(-towerTurnSpeed);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RotateTower(towerTurnSpeed);
        }
    }
}