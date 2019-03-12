using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float speed, turnSpeed, towerTurnSpeed, projectileSpeed, spinTime, attackCooldown, cameraShakeTakeDamage, cameraShakeShoot, cameraSuperShake;

    [SerializeField]
    protected int maxHealth, shotDamage, specialAttackTimer;

    [SerializeField]
    protected GameObject tankBase, tower, trackingMissile, mine;

    [SerializeField]
    protected Transform shotStart, missileStart;
    
    [SerializeField]
    protected Slider healthSlider;

    [SerializeField]
    protected AudioSource shotSound;

    [SerializeField]
    protected ParticleSystem[] frontSmoke, backSmoke, cannonParticles, healingParticles, shieldParticles;

    [SerializeField]
    bool forceSpecialAttack;    //Debugging purposes

    [SerializeField]
    [Tooltip("0 = Missile, 1 = Shield, 2 = SpeedBoost, 3 = Heal, 4 = SuperHeal, 5 = Mine, 6 = SuperShots")]
    int forcedSpecialIndex;    //Debugging purposes

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

    protected virtual void Awake()
    {
        foreach (ParticleSystem p in shieldParticles)
        {
            //ställ in particleduration
        }
        health = maxHealth;
        currentMovement = MoveTank;
        currentRotationMethod = RotateTank;
        specialAttackMethods = new SpecialAttackMethod[] { FireMissile, SpawnShield, SpeedBoost, Heal, SuperHeal, DeployMine, SuperShots };
        currentSpecialAttack = Nothing;
        canvasTF = healthSlider.gameObject.GetComponentInParent<Transform>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    private void LateUpdate()
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
        ProjectileScript shot = ProjectilePoolScript.Instance.NewProjectile().GetComponent<ProjectileScript>(); //Instantiate(projectile, shotStart.position, Quaternion.identity).GetComponent<ProjectileScript>();
        shot.gameObject.transform.position = shotStart.position;
        shot.gameObject.transform.rotation = Quaternion.identity;
        shot.Init(shotStart.transform.forward, projectileSpeed, this, shotDamage);
        shotSound.Play();
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

    public void TakeDamage(int damage)
    {
        if (!alive || shielded)
            return;
        health -= damage;
        if (health <= 0)
        {
            alive = false;
            this.health = maxHealth;
            GameManager.Instance.TankDestroyed(this);
        }
        healthSlider.value = health;
        if (this is PlayerScript)
            CameraShaker.Instance.ShakeCamera(damage * cameraShakeTakeDamage, 2f);
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
        shielded = true;
        yield return new WaitForSeconds(3);
        shielded = false;
        foreach (ParticleSystem p in shieldParticles)
        {
            p.Stop();
        }
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
        Instantiate(mine, transform.position, Quaternion.identity);
    }

    protected void FireMissile()
    {
        CameraShaker.Instance.ShakeCamera(2 * cameraShakeShoot, 2f);
        missileStart.GetComponent<AudioSource>().Play();
        GameObject missileGO = Instantiate(trackingMissile, missileStart.position, tower.transform.rotation);
        MissileScript missile = missileGO.GetComponent<MissileScript>();
        if (this is PlayerScript)
        {
            NpcScript[] enemies = FindObjectsOfType<NpcScript>();
            if (enemies.Length > 0)
            {
                missile.Init(enemies[Random.Range(0, enemies.Length)], this);
            }
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