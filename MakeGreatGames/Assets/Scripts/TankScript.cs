using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float turnSpeed, towerTurnSpeed, projectileSpeed;

    [SerializeField]
    protected int health, shotDamage, specialAttackDamage;

    [SerializeField]
    protected GameObject tankBase, tower, projectile;

    [SerializeField]
    protected Transform shotStart;

    protected bool alive = true;

    protected float charge = 0, maxCharge = 100;

    protected delegate void MovementMethod(float amount);

    protected delegate void RotationMethod(float amount);

    protected RotationMethod currentRotationMethod;

    protected MovementMethod currentMovement;

    protected virtual void Awake()
    {
        currentMovement = MoveTank;
        currentRotationMethod = RotateTank;
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

    public void Shoot()
    {
        if (!alive)
            return;
        ProjectileScript shot = Instantiate(projectile, shotStart.position, Quaternion.identity).GetComponent<ProjectileScript>();
        shot.Init(tower.transform.forward, projectileSpeed, this, shotDamage);
    }

    public void SpecialAttack()
    {
        if (!alive)
            return;

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
}