using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    Vector3 direction = Vector3.zero;
    float speed = 0.0f;
    TankScript parent;
    int damage;

    public void Init(Vector3 direction, float speed, TankScript parent, int damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.parent = parent;
        this.damage = damage;
    }
    
    void Update()
    {
        transform.Translate(direction * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TankScript hitTank = collision.gameObject.GetComponent<TankScript>();
        if(hitTank != null && hitTank != parent)
        {
            hitTank.TakeDamage(damage);
        }
    }
}
