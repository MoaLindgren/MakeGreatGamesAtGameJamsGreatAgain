using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float speed, turnSpeed, towerTurnSpeed, projectileSpeed, spinTime, attackCooldown;

    [SerializeField]
    protected int maxHealth, shotDamage, specialAttackDamage;

    [SerializeField]
    protected GameObject tankBase, tower, projectile, trackingMissile;

    [SerializeField]
    protected Transform shotStart, missileStart;

    [SerializeField]
    protected AudioClip shotSound, movementSound, deathSound;

    [SerializeField]
    protected Slider healthSlider;

    protected bool alive = true, shielded = false, canShoot = true;

    protected int health, coins = 0, maxCoins = 10;

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
        health = maxHealth;
        currentMovement = MoveTank;
        currentRotationMethod = RotateTank;
        specialAttackMethods = new SpecialAttackMethod[] { FireMissile, SpawnShield, SpeedBoost, Heal, SuperHeal };
        currentSpecialAttack = Nothing;
        canvasTF = healthSlider.gameObject.GetComponentInParent<Transform>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    private void LateUpdate()
    {
        canvasTF.LookAt(GameManager.Instance.Cam.transform);
        canvasTF.rotation = new Quaternion(0f, 0f, canvasTF.rotation.z, canvasTF.rotation.w);
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
        ProjectileScript shot = Instantiate(projectile, shotStart.position, Quaternion.identity).GetComponent<ProjectileScript>();
        shot.Init(shotStart.transform.forward, projectileSpeed, this, shotDamage);
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
        //spela spinnljud && snurra hjulen
        yield return new WaitForSeconds(spinTime);
        int specialAttackIndex = Random.Range(0, specialAttackMethods.Length);
        specialAttackMethods[specialAttackIndex]();
    }

    public void TakeDamage(int damage)
    {
        if (!alive || shielded)
            return;
        health -= damage;
        if (health <= 0)
        {
            alive = false;
            GameManager.Instance.TankDestroyed(this);
        }
        healthSlider.value = health;
    }

    #region SpecialAttacks

    protected IEnumerator SpecialAttackTimer()
    {
        yield return new WaitForSeconds(30);
        currentSpecialAttack = Nothing;
    }

    protected IEnumerator Shield()
    {
        shielded = true;
        yield return new WaitForSeconds(3);
        shielded = false;
    }

    protected IEnumerator SpeedBoosted()
    {
        float originalSpeed = speed;
        speed *= 2;
        yield return new WaitForSeconds(10);
        speed = originalSpeed;
    }

    protected IEnumerator SuperShotTimer()
    {
        int originalDamage = shotDamage;
        shotDamage *= 3;
        yield return new WaitForSeconds(10);
        shotDamage = originalDamage;
    }

    protected void FireMissile()
    {
        GameObject missileGO = Instantiate(trackingMissile, missileStart);
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
        //shieldparticles wooooooo
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
        health += 30;
        if (health > maxHealth)
            health = maxHealth;
        healthSlider.value = health;
    }

    protected void SuperHeal()
    {
        health = maxHealth;
        healthSlider.value = health;
    }

    protected void SuperShots()
    {
        StartCoroutine("SuperShotTimer");
    }

    #endregion

}