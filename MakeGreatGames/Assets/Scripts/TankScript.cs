using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    AudioClip shotSound, movementSound, deathSound;

    protected bool alive = true, shielded = false, canShoot = true;
    
    protected int health, coins = 0, maxCoins = 10;

    protected delegate void MovementMethod(float amount);

    protected delegate void RotationMethod(float amount);

    protected delegate void SpecialAttackMethod();

    protected RotationMethod currentRotationMethod;

    protected MovementMethod currentMovement;

    protected SpecialAttackMethod currentSpecialAttack;

    protected SpecialAttackMethod[] specialAttackMethods;

    protected virtual void Awake()
    {
        health = maxHealth;
        currentMovement = MoveTank;
        currentRotationMethod = RotateTank;
        specialAttackMethods = new SpecialAttackMethod[] { FireMissile, SpawnShield, SpeedBoost, Heal, SuperHeal };
        currentSpecialAttack = Nothing;
    }

    public virtual void AddCoin()
    {
        coins++;
    }

    protected void RotateTower(float amount)
    {
        if (!alive)
            return;
        //tower.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + amount, transform.rotation.z, transform.rotation.w);
        tower.transform.Rotate(0f, amount, 0f);
    }

    protected void RotateTank(float amount)
    {
        if (!alive)
            return;
        //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + amount, transform.rotation.z, transform.rotation.w);
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
        if (!alive)
            return;
        ProjectileScript shot = Instantiate(projectile, shotStart.position, Quaternion.identity).GetComponent<ProjectileScript>();
        shot.Init(tower.transform.forward, projectileSpeed, this, shotDamage);
        StartCoroutine("AttackCooldown");
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
            if (enemies.Length < 1)
            {
                //åk nånstans random o boom
            }
            else
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
    }

    protected void SuperHeal()
    {
        health = maxHealth;
    }

    protected void SuperShots()
    {
        StartCoroutine("SuperShotTimer");
    }

    #endregion

}