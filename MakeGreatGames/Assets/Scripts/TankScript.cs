﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float speed, turnSpeed, towerTurnSpeed, projectileSpeed, spinTime, attackCooldown, cameraShakeTakeDamage, cameraShakeShoot, cameraSuperShake;

    [SerializeField]
    protected int maxHealth, shotDamage, specialAttackDamage, specialAttackTimer;

    [SerializeField]
    protected GameObject tankBase, tower, projectile, trackingMissile, mine;

    [SerializeField]
    protected Transform shotStart, missileStart;

    [SerializeField]
    protected AudioClip movementSound, deathSound, missileLaunchSound;

    [SerializeField]
    protected Slider healthSlider;

    [SerializeField]
    protected AudioSource shotSound;

    [SerializeField]
    protected ParticleSystem[] frontSmoke, backSmoke, cannonParticles, healingParticles;

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

    protected Vector3 lastPos;

    protected virtual void Awake()
    {
        foreach(ParticleSystem p in frontSmoke)
        {
            p.Stop();
        }
        foreach (ParticleSystem p in backSmoke)
        {
            p.Stop();
        }
        foreach (ParticleSystem p in cannonParticles)
        {
            p.Stop();
        }
        foreach (ParticleSystem p in healingParticles)
        {
            p.Stop();
        }
        lastPos = transform.position;
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
        ProjectileScript shot = Instantiate(projectile, shotStart.position, Quaternion.identity).GetComponent<ProjectileScript>();
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
        //spela spinnljud && snurra hjulen
        yield return new WaitForSeconds(spinTime);
        specialAttackIndex = Random.Range(0, specialAttackMethods.Length);
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
        shielded = true;
        yield return new WaitForSeconds(3);
        shielded = false;
    }

    protected virtual IEnumerator SpeedBoosted()
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

    protected void DeployMine()
    {
        Instantiate(mine, transform.position, Quaternion.identity);
    }

    protected void FireMissile()
    {
        print("NUKEM");
        CameraShaker.Instance.ShakeCamera(2 * cameraShakeShoot, 2f);
        missileStart.GetComponent<AudioSource>().Play();
        GameObject missileGO = Instantiate(trackingMissile, missileStart.position, Quaternion.identity);
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
        print("sköld yao");
        StartCoroutine("Shield");
    }

    protected void Nothing()
    {
        return;
    }

    protected void SpeedBoost()
    {
        print("VROOOOOOM");
        StartCoroutine("SpeedBoosted");
    }

    protected void Heal()
    {
        foreach (ParticleSystem p in healingParticles)
        {
            p.Play();
        }
        print("healing");
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
        print("SUPERHEEEEEAL");
        health = maxHealth;
        healthSlider.value = health;
    }

    protected void SuperShots()
    {
        print("i kill you");
        StartCoroutine("SuperShotTimer");
    }

    #endregion

}