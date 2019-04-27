using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TankScript : NetworkBehaviour
{
    [SerializeField]
    protected float spinTime, attackCooldown, cameraShakeTakeDamage, cameraShakeShoot, cameraSuperShake, shieldTime, destroyTimer;

    [SerializeField]
    protected int specialAttackTimer;

    [SerializeField]
    protected GameObject tankBase, tower, trackingMissile, mine, audioListener;

    [SerializeField]
    protected Transform shotStart, missileStart;

    [SerializeField]
    protected Slider healthSlider, healthSliderDelayed;

    [SerializeField]
    protected ParticleSystem[] frontSmoke, backSmoke, cannonParticles, healingParticles, shieldParticles, superShotParticles;

    [SerializeField]
    protected bool forceSpecialAttack;    //Debugging purposes

    [SerializeField]
    [Tooltip("0 = Missile, 1 = Shield, 2 = SpeedBoost, 3 = Heal, 4 = SuperHeal, 5 = Mine, 6 = SuperShots")]
    protected int forcedSpecialIndex;    //Debugging purposes

    protected bool alive = true, shielded = false, canShoot = true, spinning = false, onNetwork = false;

    protected int maxHealth, shotDamage, health, coins = 0, maxCoins = 10, specialAttackIndex, targetHealth;

    protected delegate void MovementMethod(float amount);

    protected delegate void RotationMethod(float amount);

    protected delegate void SpecialAttackMethod();

    protected RotationMethod currentRotationMethod;

    protected MovementMethod currentMovement;

    protected SpecialAttackMethod currentSpecialAttack;

    protected SpecialAttackMethod[] specialAttackMethods;

    protected Transform directSliderTF, delayedSliderTF;

    protected AudioSource engineSound;

    protected float speed, maxSpeed, turnSpeed, towerTurnSpeed, projectileSpeed, acceleration, ultStartedTime = 0f;

    protected Camera targetCam;

    public bool OnNetwork
    {
        get { return onNetwork; }
    }

    protected virtual void Awake()
    {
        onNetwork = GetComponent<NetworkIdentity>() != null;
    }

    protected virtual void Start()
    {
        currentMovement = MoveTank;
        currentRotationMethod = RotateTank;
        specialAttackMethods = new SpecialAttackMethod[] { FireMissile, SpawnShield, SpeedBoost, Heal, SuperHeal, DeployMine, SuperShots };
        currentSpecialAttack = Nothing;
        directSliderTF = healthSlider.gameObject.GetComponentInParent<Transform>();
        delayedSliderTF = healthSliderDelayed.gameObject.GetComponentInParent<Transform>();
        InitializeStats();
    }

    protected void InitializeStats()
    {
        Transform baseTransform = transform.Find("Tank_Base");
        string baseName = this is PlayerScript ? baseTransform.GetComponent<MeshFilter>().sharedMesh.name : "Enemy_Base", towerName = this is PlayerScript ? baseTransform.Find("Tank_Tower").GetComponent<MeshFilter>().sharedMesh.name : "Enemy_Tower";
        maxHealth = GameManager.Instance.GetTankStat<int>("Base", baseName, "MaxHealth");
        turnSpeed = GameManager.Instance.GetTankStat<float>("Base", baseName, "TurnSpeed");
        acceleration = GameManager.Instance.GetTankStat<float>("Base", baseName, "Acceleration");
        maxSpeed = GameManager.Instance.GetTankStat<float>("Base", baseName, "MaxSpeed");
        towerTurnSpeed = GameManager.Instance.GetTankStat<float>("Tower", towerName, "TurnSpeed");
        projectileSpeed = GameManager.Instance.GetTankStat<float>("Tower", towerName, "ProjectileSpeed");
        attackCooldown = GameManager.Instance.GetTankStat<float>("Tower", towerName, "ShotCooldown");
        shotDamage = GameManager.Instance.GetTankStat<int>("Tower", towerName, "Damage");
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        healthSliderDelayed.maxValue = maxHealth;
        healthSliderDelayed.value = health;
        targetHealth = maxHealth;
        targetCam = GameManager.Instance.GetCam();
        if(!onNetwork || isLocalPlayer)
        {
            Instantiate(audioListener, tankBase.transform);
        }
    }

    protected virtual void Update()
    {
        healthSliderDelayed.value = Mathf.Lerp(healthSliderDelayed.value, targetHealth, targetHealth == 0 ? 0.01f : Mathf.Abs((targetHealth / 100f) / (healthSliderDelayed.value / 100f)) / 50f);
        if (healthSliderDelayed.value < targetHealth || healthSliderDelayed.value - targetHealth < 0.1f)
        {
            healthSliderDelayed.value = targetHealth;
        }
    }

    protected void LateUpdate()
    {
        if (onNetwork && !isLocalPlayer)
            return;
        directSliderTF.LookAt(directSliderTF.position + targetCam.transform.rotation * Vector3.forward, targetCam.transform.rotation * Vector3.up);
        delayedSliderTF.LookAt(directSliderTF.position + targetCam.transform.rotation * Vector3.forward, targetCam.transform.rotation * Vector3.up);
    }

    public virtual void AddCoin()
    {
        coins++;
    }
    
    protected void RotateTower(float amount)
    {
        if (!alive)
            return;
        float rotationCompensation = this is PlayerScript ? (this as PlayerScript).RotationCompensation : 0f;
        tower.transform.Rotate(0f, amount - rotationCompensation * 0.28f, 0f);
    }
    
    protected void RotateTank(float amount)
    {
        if (!alive)
            return;
        transform.Rotate(0f, amount, 0f);
    }

    protected virtual void MoveTank(float amount)
    {
        if (!alive)
            return;
        if (engineSound != null && engineSound.clip != null)
            engineSound.pitch = 0.95f + ((Mathf.Abs(amount) / (maxSpeed / 100f)) / 500f);
    }

    protected void DontMoveTank(float amount)
    {
        return;
    }
    
    public void Shoot()
    {
        if (!alive || !canShoot)
            return;
        foreach (ParticleSystem p in cannonParticles)
        {
            p.Play();
        }
        ProjectileScript shot = GameManager.Instance.ProjectilePool.GetObject(shotStart.position, shotStart.rotation).GetComponent<ProjectileScript>();
        shot.Init(shotStart.transform.forward, projectileSpeed, this, shotDamage);
        AudioSource shotSound = AudioManager.Instance.SpawnSound("ShotSound", shotStart, false, false, false, this is PlayerScript ? 1f : 0.8f);
        StopCoroutine("AttackCooldownTimer");
        StartCoroutine("AttackCooldownTimer");
    }

    protected IEnumerator AttackCooldownTimer()
    {
        canShoot = false;
        yield return new WaitForSeconds(attackCooldown);
        canShoot = true;
    }

    protected IEnumerator SpinWheel()
    {
        if (!alive)
            StopCoroutine("SpinWheel");
        spinning = true;
        if (this is PlayerScript)
        {
            UIManager.Instance.SpinWheel();
        }
        yield return new WaitForSeconds(spinTime);
        spinning = false;
        specialAttackIndex = forceSpecialAttack ? forcedSpecialIndex : Random.Range(0, specialAttackMethods.Length);
        StartCoroutine("SpecialAttackTimer");
        currentSpecialAttack = specialAttackMethods[specialAttackIndex];
        if (this is PlayerScript)
        {
            UIManager.Instance.SpecialAttack(true, specialAttackTimer, specialAttackIndex);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (!alive || shielded)
            return;
        health -= damage;
        healthSlider.value = health;
        StartCoroutine("WaitForDamage");
        if (this is PlayerScript)
        {
            UIManager.Instance.ShowDamage(health, maxHealth);
            CameraShaker.Instance.ShakeCamera(damage * cameraShakeTakeDamage, 2f);
        }
        if (health <= 0)
        {
            if (engineSound != null && engineSound.clip != null)
                engineSound.pitch = 1f;
            health = 0;
            alive = false;
            StartCoroutine("DestroyTimer");
        }
    }

    protected IEnumerator WaitForDamage()
    {
        yield return new WaitForSeconds(0.7f);
        targetHealth = health;
    }

    protected IEnumerator DestroyTimer()
    {
        //deathsound, sfx & animation here
        yield return new WaitForSeconds(destroyTimer);
        this.health = maxHealth;
        GameManager.Instance.TankDestroyed(this);
    }

    #region SpecialAttacks

    protected IEnumerator SpecialAttackTimer()
    {
        ultStartedTime = Time.time;
        yield return new WaitForSeconds(specialAttackTimer);
        currentSpecialAttack = Nothing;
    }

    protected IEnumerator Shield()
    {
        foreach (ParticleSystem p in shieldParticles)
        {
            p.Play();
        }
        AudioSource shieldSound = AudioManager.Instance.SpawnSound("ShieldSound", transform, false, true, false, 1f);
        shielded = true;
        yield return new WaitForSeconds(shieldTime);
        shielded = false;
        foreach (ParticleSystem p in shieldParticles)
        {
            p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        AudioManager.Instance.ReturnSource(shieldSound);
    }

    protected virtual IEnumerator SpeedBoosted()
    {
        float originalSpeed = maxSpeed;
        float originalTurnSpeed = turnSpeed;
        AudioManager.Instance.SpawnSound("SpeedBoostSound", transform, false, false, false, 1f);
        turnSpeed *= 2;
        maxSpeed *= 2;
        yield return new WaitForSeconds(10);
        turnSpeed = originalTurnSpeed;
        maxSpeed = originalSpeed;
    }

    protected IEnumerator SuperShotTimer()
    {
        foreach (ParticleSystem p in superShotParticles)
            p.Play();
        AudioSource superShotSound = AudioManager.Instance.SpawnSound("SuperShotSound", transform, false, true, false, 1f);
        int originalDamage = shotDamage;
        shotDamage *= 3;
        yield return new WaitForSeconds(10);
        shotDamage = originalDamage;
        AudioManager.Instance.ReturnSource(superShotSound);
        foreach (ParticleSystem p in superShotParticles)
            p.Stop();
    }

    protected IEnumerator SuperHealing()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(1f);
            health = Mathf.Clamp(health + (maxHealth / 20), 0, maxHealth);
            healthSlider.value = health;
            if (this is PlayerScript)
                UIManager.Instance.ShowDamage(health, maxHealth);
        }
    }
    
    protected void DeployMine()
    {
        GameManager.Instance.MinePool.GetObject(transform.position, Quaternion.identity);
    }
    
    protected void FireMissile()
    {
        CameraShaker.Instance.ShakeCamera(2 * cameraShakeShoot, 2f);
        AudioManager.Instance.SpawnSound("MissileLaunchSound", missileStart.transform, false, false, false, this is PlayerScript ? 0.7f : 0.6f);
        GameObject missileGO = GameManager.Instance.MissilePool.GetObject(missileStart.position, tower.transform.rotation);
        MissileScript missile = missileGO.GetComponent<MissileScript>();
        if (this is PlayerScript)
        {
            if (WaveSpawner.Instance.CurrentWaveTanks.Count > 0)
            {
                missile.Init(WaveSpawner.Instance.CurrentWaveTanks[Random.Range(0, WaveSpawner.Instance.CurrentWaveTanks.Count)].GetComponent<TankScript>(), this);
            }
            else
                missile.Init(null, this);
        }
        else
        {
            missile.Init(FindObjectOfType<PlayerScript>(), this);
        }
    }
    
    protected void SpawnShield()
    {
        StartCoroutine("Shield");
    }

    protected void Nothing()
    {
        return;
    }
    
    protected void SpeedBoost()
    {
        StartCoroutine("SpeedBoosted");
    }
    
    protected void Heal()
    {
        AudioSource healingSound = AudioManager.Instance.SpawnSound("HealingSound", transform, false, false, false, 1f);
        foreach (ParticleSystem p in healingParticles)
        {
            p.Play();
        }
        health += (maxHealth / 100) * 30;
        if (health > maxHealth)
            health = maxHealth;
        healthSlider.value = health;
        if (this is PlayerScript)
            UIManager.Instance.ShowDamage(health, maxHealth);
    }

    protected void SuperHeal()
    {
        AudioSource healingSound = AudioManager.Instance.SpawnSound("SuperHealingSound", transform, false, false, false, 1f);
        foreach (ParticleSystem p in healingParticles)
        {
            p.Play();
        }
        StartCoroutine("SuperHealing");
    }

    protected void SuperShots()
    {
        StartCoroutine("SuperShotTimer");
    }

    #endregion

}