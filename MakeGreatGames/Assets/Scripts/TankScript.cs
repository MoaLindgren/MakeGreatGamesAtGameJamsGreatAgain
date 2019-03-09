using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float turnSpeed, towerTurnSpeed, projectileSpeed;

    [SerializeField]
    int health, shotDamage, specialAttackDamage;

    [SerializeField]
    protected GameObject tankBase, tower, projectile;

    [SerializeField]
    Transform shotStart;

    float charge = 0, maxCharge = 100;

    Rigidbody rB;

    protected delegate void MovementMethod(float amount);

    protected delegate void RotationMethod(float amount);

    protected RotationMethod currentRotationMethod;

    protected MovementMethod currentMovement;

    protected virtual void Awake()
    {
        rB = GetComponent<Rigidbody>();
        currentMovement = MoveTank;
        currentRotationMethod = RotateTank;
    }

    protected void RotateTower(float amount)
    {

    }

    protected void RotateTank(float amount)
    {
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + amount, transform.rotation.z, transform.rotation.w);
    }

    protected void MoveTank(float amount)
    {
        rB.AddForce(transform.forward * amount);
    }

    public void Shoot()
    {
        ProjectileScript shot = Instantiate(projectile, shotStart.position, Quaternion.identity).GetComponent<ProjectileScript>();
        shot.Init(tower.transform.forward, projectileSpeed, this);
    }

    public void SpecialAttack()
    {

    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManager.Instance.TankDestroyed(this);
        }
    }
}