using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float speed, turnSpeed, towerTurnSpeed, projectileSpeed, spinTime, attackCooldown, cameraShakeTakeDamage, cameraShakeShoot, cameraSuperShake, shieldTime, destroyTimer;

    [SerializeField]
    protected int maxHealth, shotDamage, specialAttackTimer;

    [SerializeField]
    protected GameObject tankBase, tower, trackingMissile, mine;

    [SerializeField]
    protected Transform shotStart, missileStart;

    [SerializeField]
    protected Slider healthSlider;
    
    [SerializeField]
    protected ParticleSystem[] frontSmoke, backSmoke, cannonParticles, healingParticles, shieldParticles;

    [SerializeField]
    protected bool forceSpecialAttack;    //Debugging purposes

    [SerializeField]
    [Tooltip("0 = Missile, 1 = Shield, 2 = SpeedBoost, 3 = Heal, 4 = SuperHeal, 5 = Mine, 6 = SuperShots")]
    protected int forcedSpecialIndex;    //Debugging purposes

    protected bool alive = true, shielded = false, canShoot = true;

    protected int health, coins = 0, maxCoins = 10, specialAttackIndex;

    protected delegate void MovementMethod(float amount);

    protected delegate void RotationMethod(float amount);

    protected delegate void SpecialAttackMethod();

    protected RotationMethod currentRotationMethod;

    protected MovementMethod currentMovement;

    protected SpecialAttackMethod currentSpecialAttack;

    protected SpecialAttackMethod[] specialAttackMethods;

    protected Transform canvasTF;

    protected AudioSource engineSound;

    protected virtual void Awake()
    {
        health = maxHealth;
        currentMovement = MoveTank;
        currentRotationMethod = RotateTank;
        specialAttackMethods = new SpecialAttackMethod[] { FireMissile, SpawnShield, SpeedBoost, Heal, SuperHeal, DeployMine, SuperShots };
        currentSpecialAttack = Nothing;
        canvasTF = healthSlider.gameObject.GetComponentInParent<Transform>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    protected void LateUpdate()
    {
        canvasTF.LookAt(canvasTF.position + GameManager.Instance.Cam.transform.rotation * Vector3.forward, GameManager.Instance.Cam.transform.rotation * Vector3.up);
    }

    public virtual void AddCoin()
    {
        coins++;
    }

    protected void RotateTower(float amount)
    {
        if (!alive)
            return;
        tower.transform.Rotate(0f, amount, 0f);
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
        AudioSource shotSound = AudioManager.Instance.SpawnSound("ShotSound", shotStart, false, false, false, this is PlayerScript ? 0.85f : 0.4f);
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
        if (this is PlayerScript)
        {
            UiManager.Instance.SpinWheel();
        }
        yield return new WaitForSeconds(spinTime);
        specialAttackIndex = forceSpecialAttack ? forcedSpecialIndex : Random.Range(0, specialAttackMethods.Length);
        currentSpecialAttack = specialAttackMethods[specialAttackIndex];
        if (this is PlayerScript)
        {
            UiManager.Instance.SpecialAttack(true, specialAttackTimer, specialAttackIndex);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (!alive || shielded)
            return;
        health -= damage;
        healthSlider.value = health;
        if (health <= 0)
        {
            health = 0;
            alive = false;
            StartCoroutine("DestroyTimer");
        }
        if (this is PlayerScript)
            CameraShaker.Instance.ShakeCamera(damage * cameraShakeTakeDamage, 2f);
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
            p.Stop();
        }
        AudioManager.Instance.ReturnSource(shieldSound);
    }

    protected virtual IEnumerator SpeedBoosted()
    {
        float originalSpeed = speed;
        float originalTurnSpeed = turnSpeed;
        turnSpeed *= 2;
        speed *= 2;
        yield return new WaitForSeconds(10);
        turnSpeed = originalTurnSpeed;
        speed = originalSpeed;
    }

    protected IEnumerator SuperShotTimer()
    {
        int originalDamage = shotDamage;
        shotDamage *= 3;
        yield return new WaitForSeconds(10);
        shotDamage = originalDamage;
    }

    protected void DeployMine()
    {
        GameManager.Instance.MinePool.GetObject(transform.position, Quaternion.identity);
    }

    protected void FireMissile()
    {
        CameraShaker.Instance.ShakeCamera(2 * cameraShakeShoot, 2f);
        AudioManager.Instance.SpawnSound("MissileLaunchSound", missileStart.transform, false, false, false, this is PlayerScript ? 0.8f : 0.6f);
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
        health += 30;
        if (health > maxHealth)
            health = maxHealth;
        healthSlider.value = health;
    }

    protected void SuperHeal()
    {
        AudioSource healingSound = AudioManager.Instance.SpawnSound("HealingSound", transform, false, false, false, 1f);
        foreach (ParticleSystem p in healingParticles)
        {
            p.Play();
        }
        health = maxHealth;
        healthSlider.value = health;
    }

    protected void SuperShots()
    {
        StartCoroutine("SuperShotTimer");
    }

    #endregion

}