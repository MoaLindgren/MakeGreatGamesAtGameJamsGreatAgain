using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float turnSpeed, towerTurnSpeed, projectileSpeed, spinTime;

    [SerializeField]
    protected int health, shotDamage, specialAttackDamage;

    [SerializeField]
    protected GameObject tankBase, tower, projectile, trackingMissile;

    [SerializeField]
    protected Transform shotStart, missileStart;

    //[SerializeField]
    //AudioClip shotSound, movementSound, deathSound

    protected bool alive = true;

    protected float charge = 0, maxCharge = 100;

    protected delegate void MovementMethod(float amount);

    protected delegate void RotationMethod(float amount);

    protected delegate void SpecialAttackMethod();

    protected RotationMethod currentRotationMethod;

    protected MovementMethod currentMovement;

    protected SpecialAttackMethod currentSpecialAttack;

    SpecialAttackMethod[] specialAttackMethods;

    protected virtual void Awake()
    {
        currentMovement = MoveTank;
        currentRotationMethod = RotateTank;
        specialAttackMethods = new SpecialAttackMethod[] { FireMissile, SpawnShield, Nothing, SpeedBoost, Heal };
        currentSpecialAttack = Nothing;
    }

    protected void RotateTower(float amount)
    {
        if (!alive)
            return;
        tower.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + amount, transform.rotation.z, transform.rotation.w);
    }

    protected void RotateTank(float amount)
    {
        if (!alive)
            return;
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + amount, transform.rotation.z, transform.rotation.w);
    }

    protected void MoveTank(float amount)
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
    }

    protected IEnumerator SpecialAttack()
    {
        if (!alive)
            StopCoroutine("SpecialAttack");
        //spela spinnljud && snurra hjulen
        yield return new WaitForSeconds(spinTime);
        int specialAttackIndex = Random.Range(0, specialAttackMethods.Length);
        specialAttackMethods[specialAttackIndex]();
    }

    public void TakeDamage(int damage)
    {
        if (!alive)
            return;
        health -= damage;
        if (health <= 0)
        {
            alive = false;
            GameManager.Instance.TankDestroyed(this);
        }
    }

    #region SpecialAttacks

    IEnumerator SpecialAttackTimer()
    {
        yield return new WaitForSeconds(30);
        currentSpecialAttack = Nothing;
    }

    IEnumerator Shield()
    {
        yield return new WaitForSeconds(3);
    }

    void FireMissile()
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

    void SpawnShield()
    {
        //shieldparticles wooooooo

    }

    void Nothing()
    {

    }

    void SpeedBoost()
    {

    }

    void Heal()
    {

    }

    void SuperShots()
    {

    }

    #endregion
}