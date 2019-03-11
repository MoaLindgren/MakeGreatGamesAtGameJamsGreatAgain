using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    Vector3 direction = Vector3.zero;
    float speed = 0.0f;
    TankScript shooter;
    int damage;

    public void Init(Vector3 direction, float speed, TankScript parent, int damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.shooter = parent;
        this.damage = damage;
        Destroy(gameObject, 15);
    }

    void Update()
    {
        if (GameManager.Instance.Paused)
            return;
        transform.Translate(direction * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript hitTank = other.gameObject.GetComponent<TankScript>();
        if (hitTank != null && hitTank != shooter)
        {
            hitTank.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (hitTank == null || hitTank != shooter)
        {
            Destroy(gameObject);
        }
    }
}