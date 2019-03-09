using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    [SerializeField]
    protected float turnSpeed, towerTurnSpeed, projectileSpeed;

    [SerializeField]
    protected GameObject tankBase, tower, projectile;

    [SerializeField]
    Transform shotStart;

    float charge = 0;

    protected void RotateTower(float amount)
    {

    }

    protected void RotateTank(float amount)
    {

    }

    public void Shoot()
    {
        GameObject shot = Instantiate(projectile, shotStart.position, shotStart.rotation);
    }

    public void SpecialAttack()
    {

    }
}